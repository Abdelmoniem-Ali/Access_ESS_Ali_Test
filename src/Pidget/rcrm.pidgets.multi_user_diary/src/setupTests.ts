import '@testing-library/jest-dom';
import ResizeObserver from 'resize-observer-polyfill';

global.ResizeObserver = ResizeObserver; // fixes "ResizeObserver is not defined" issue in floating-ui

// fixes matchMedia not present from react-slick
window.matchMedia =
    window.matchMedia ||
    function () {
        return {
            matches: false,
            // eslint-disable-next-line @typescript-eslint/no-empty-function
            addListener: function () {},
            // eslint-disable-next-line @typescript-eslint/no-empty-function
            removeListener: function () {},
        };
    };
