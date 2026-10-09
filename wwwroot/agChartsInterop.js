const libraries = new Map();
const charts = new Map();
let communityModulesRegistered = false;
let enterpriseModulesRegistered = false;

const droppedEventKeys = new Set([
    "series",
    "chart",
    "axis",
    "node",
    "item",
    "sourceEvent",
    "event",
]);

export async function syncChart(id, container, optionsJson, settings, dotNetRef) {
    const api = await loadApi(settings);
    const options = revive(JSON.parse(optionsJson));
    delete options.container;
    options.container = container;
    attachListeners(options, settings, dotNetRef);

    const signature = `${settings.preset}|${settings.edition}`;
    const current = charts.get(id);
    if (current && current.signature === signature) {
        await current.chart.update(options);
        return;
    }

    current?.chart.destroy();
    const chart = createChart(api, settings.preset, options);
    charts.set(id, { chart, signature });
    if (typeof chart.waitForUpdate === "function") {
        await chart.waitForUpdate();
    }
}

export function destroyChart(id) {
    const current = charts.get(id);
    if (!current) {
        return;
    }

    current.chart.destroy();
    charts.delete(id);
}

export async function updateDelta(id, optionsJson) {
    const options = revive(JSON.parse(optionsJson));
    delete options.container;
    await requireChart(id).updateDelta(options);
}

export function applyTransaction(id, transactionJson) {
    return requireChart(id).applyTransaction(revive(JSON.parse(transactionJson)));
}

export function downloadChart(id, options) {
    return requireChart(id).download(cleanImageOptions(options));
}

export function getImageDataUrl(id, options) {
    return requireChart(id).getImageDataURL(cleanImageOptions(options));
}

export function getChartState(id) {
    return JSON.stringify(requireChart(id).getState());
}

export function setChartState(id, stateJson) {
    requireChart(id).setState(JSON.parse(stateJson));
}

export function waitForChartUpdate(id) {
    return requireChart(id).waitForUpdate();
}

function requireChart(id) {
    const current = charts.get(id);
    if (!current) {
        throw new Error("AG Charts has not created this chart yet.");
    }

    return current.chart;
}

function createChart(api, preset, options) {
    const chartsApi = api.AgCharts;
    if (!chartsApi) {
        throw new Error("AG Charts loaded without an AgCharts export.");
    }

    switch (preset) {
        case "financial":
            return chartsApi.createFinancialChart(options);
        case "gauge":
            return chartsApi.createGauge(options);
        case "quadrant":
            return chartsApi.createQuadrantChart(options);
        default:
            return chartsApi.create(options);
    }
}

async function loadApi(settings) {
    const needsEnterprise = settings.edition === "enterprise" || settings.preset !== "chart";
    const url = needsEnterprise ? settings.enterpriseModuleUrl : settings.communityModuleUrl;
    const globalNs = globalThis.agCharts;
    const globalMatches = !!globalNs?.AgCharts
        && (!needsEnterprise || !!globalNs.AllEnterpriseModule || !!globalNs.LicenseManager);
    const ns = globalMatches ? globalNs : await loadLibrary(url);

    ensureModules(ns, needsEnterprise);
    if (needsEnterprise && settings.licenseKey && typeof ns.LicenseManager?.setLicenseKey === "function") {
        ns.LicenseManager.setLicenseKey(settings.licenseKey);
    }

    return ns;
}

function loadLibrary(url) {
    if (!libraries.has(url)) {
        libraries.set(url, importLibrary(url));
    }

    return libraries.get(url);
}

async function importLibrary(url) {
    if (isEsmUrl(url)) {
        return import(/* @vite-ignore */ url);
    }

    await loadScript(url);
    if (!globalThis.agCharts?.AgCharts) {
        throw new Error(`AG Charts loaded from ${url}, but window.agCharts.AgCharts was not found.`);
    }

    return globalThis.agCharts;
}

function isEsmUrl(url) {
    return url.includes("+esm") || url.endsWith(".mjs") || url.includes("/esm/");
}

function loadScript(url) {
    return new Promise((resolve, reject) => {
        const selector = `script[data-ag-charts-src="${cssEscape(url)}"]`;
        const existing = document.querySelector(selector);
        if (existing?.dataset.loaded === "true") {
            resolve();
            return;
        }

        if (existing) {
            existing.addEventListener("load", () => resolve(), { once: true });
            existing.addEventListener("error", () => reject(new Error(`Failed to load AG Charts from ${url}.`)), { once: true });
            return;
        }

        const script = document.createElement("script");
        script.src = url;
        script.async = true;
        script.dataset.agChartsSrc = url;
        script.addEventListener("load", () => {
            script.dataset.loaded = "true";
            resolve();
        }, { once: true });
        script.addEventListener("error", () => reject(new Error(`Failed to load AG Charts from ${url}.`)), { once: true });
        document.head.appendChild(script);
    });
}

function cssEscape(value) {
    return window.CSS?.escape ? window.CSS.escape(value) : value.replace(/["\\]/g, "\\$&");
}

function ensureModules(ns, needsEnterprise) {
    if (needsEnterprise) {
        if (enterpriseModulesRegistered) {
            return;
        }

        if (ns.AllEnterpriseModule && typeof ns.ModuleRegistry?.registerModules === "function") {
            ns.ModuleRegistry.registerModules([ns.AllEnterpriseModule]);
        }

        enterpriseModulesRegistered = true;
        return;
    }

    if (communityModulesRegistered || enterpriseModulesRegistered) {
        return;
    }

    if (ns.AllCommunityModule && typeof ns.ModuleRegistry?.registerModules === "function") {
        ns.ModuleRegistry.registerModules([ns.AllCommunityModule]);
    }

    communityModulesRegistered = true;
}

function attachListeners(options, settings, dotNetRef) {
    if (!dotNetRef) {
        return;
    }

    const listeners = { ...(options.listeners ?? {}) };
    wire(listeners, "click", settings.click, "OnClickFromJs");
    wire(listeners, "doubleClick", settings.doubleClick, "OnDoubleClickFromJs");
    wire(listeners, "seriesNodeClick", settings.seriesNodeClick, "OnSeriesNodeClickFromJs");
    wire(listeners, "seriesNodeDoubleClick", settings.seriesNodeDoubleClick, "OnSeriesNodeDoubleClickFromJs");
    if (Object.keys(listeners).length > 0) {
        options.listeners = listeners;
    }

    function wire(target, eventName, enabled, method) {
        if (!enabled) {
            return;
        }

        const prior = target[eventName];
        target[eventName] = (event) => {
            if (typeof prior === "function") {
                prior(event);
            }

            const payload = sanitize(event);
            Promise.resolve(dotNetRef.invokeMethodAsync(method, payload)).catch((error) => {
                console.error("AG Charts event callback failed.", error);
            });
        };
    }
}

function revive(value) {
    if (Array.isArray(value)) {
        return value.map(revive);
    }

    if (!value || typeof value !== "object") {
        return value;
    }

    const keys = Object.keys(value);
    if (keys.length === 1 && keys[0] === "$js" && typeof value.$js === "string") {
        const compiled = new Function(`"use strict"; return (${value.$js});`)();
        if (typeof compiled !== "function") {
            throw new Error("JsCallback must compile to a function.");
        }

        return compiled;
    }

    const copy = {};
    for (const key of keys) {
        copy[key] = revive(value[key]);
    }

    return copy;
}

function sanitize(event) {
    if (!event || typeof event !== "object") {
        return {};
    }

    const seen = new WeakSet();
    const json = JSON.stringify(event, (key, value) => {
        if (droppedEventKeys.has(key)) {
            return undefined;
        }

        if (typeof value === "function" || typeof value === "symbol") {
            return undefined;
        }

        if (typeof Element !== "undefined" && value instanceof Element) {
            return undefined;
        }

        if (value && typeof value === "object") {
            if (seen.has(value)) {
                return undefined;
            }

            seen.add(value);
        }

        return value;
    });

    return json ? JSON.parse(json) : {};
}

function cleanImageOptions(options) {
    if (!options) {
        return undefined;
    }

    const clean = {};
    if (options.fileName) {
        clean.fileName = options.fileName;
    }

    if (options.width) {
        clean.width = options.width;
    }

    if (options.height) {
        clean.height = options.height;
    }

    return clean;
}
