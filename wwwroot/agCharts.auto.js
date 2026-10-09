import { syncChart } from "./agChartsInterop.js";

const selector = "[data-ag-chart]";

export function scan(root = document) {
    root.querySelectorAll(selector).forEach((element) => {
        if (element.dataset.agChartReady === "true") {
            return;
        }

        const options = document.querySelector(`script[data-ag-chart-options="${cssEscape(element.id)}"]`);
        const optionsJson = options?.textContent?.trim() || "{}";
        const preset = element.dataset.agChartPreset || "chart";
        const edition = element.dataset.agChartEdition || "community";
        const version = element.dataset.agChartVersion;
        const settings = {
            preset,
            edition,
            version,
            licenseKey: element.dataset.agChartLicense || null,
            communityModuleUrl: element.dataset.agChartCommunityModule
                || `https://cdn.jsdelivr.net/npm/ag-charts-community@${version || "14.2.0"}/dist/umd/ag-charts-community.min.js`,
            enterpriseModuleUrl: element.dataset.agChartEnterpriseModule
                || `https://cdn.jsdelivr.net/npm/ag-charts-enterprise@${version || "14.2.0"}/dist/umd/ag-charts-enterprise.min.js`,
            click: !!element.dataset.agChartClick,
            doubleClick: !!element.dataset.agChartDoubleClick,
            seriesNodeClick: !!element.dataset.agChartSeriesNodeClick,
            seriesNodeDoubleClick: !!element.dataset.agChartSeriesNodeDoubleClick,
        };

        element.dataset.agChartReady = "true";
        syncChart(element.id, element, optionsJson, settings, createPageBridge(element)).catch((error) => {
            element.dataset.agChartReady = "false";
            showError(element, error);
        });
    });
}

function createPageBridge(element) {
    return {
        invokeMethodAsync(method, payload) {
            const names = {
                OnSeriesNodeClickFromJs: element.dataset.agChartSeriesNodeClick,
                OnSeriesNodeDoubleClickFromJs: element.dataset.agChartSeriesNodeDoubleClick,
                OnClickFromJs: element.dataset.agChartClick,
                OnDoubleClickFromJs: element.dataset.agChartDoubleClick,
            };
            const name = names[method];
            const callback = name ? window[name] : null;
            if (typeof callback === "function") {
                callback(payload);
            }
        },
    };
}

function showError(element, error) {
    console.error(error);
    const message = document.createElement("div");
    message.className = "ag-chart-error";
    message.setAttribute("role", "alert");
    message.textContent = error?.message || "AG Charts failed to render.";
    element.replaceChildren(message);
}

function cssEscape(value) {
    return window.CSS?.escape ? window.CSS.escape(value) : value.replace(/["\\]/g, "\\$&");
}

globalThis.AgChartsRazor = { scan };
scan();
