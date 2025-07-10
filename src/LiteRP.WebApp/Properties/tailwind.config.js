/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "../wwwroot/**/*.{razor,html,cshtml,cs}",
        "../Components/*.{razor,html,cshtml,cs}",
        "../Components/**/*.{razor,html,cshtml,cs}"
    ],
    darkMode: 'class',
    safelist: [
        "md:bg-transparent",
        "md:block",
        "md:border-0",
        "md:dark:hover:bg-transparent",
        "md:dark:hover:text-white",
        "md:flex-row",
        "md:font-medium",
        "md:hidden",
        "md:hover:bg-transparent",
        "md:hover:text-primary-700",
        "md:mt-0",
        "md:p-0",
        "md:space-x-8",
        "md:text-primary-700",
        "md:text-sm",
        "md:w-auto"
    ],
    theme: {
        extend: {
            colors: {
                primary: { "50": "#eff6ff", "100": "#dbeafe", "200": "#bfdbfe", "300": "#93c5fd", "400": "#60a5fa", "500": "#3b82f6", "600": "#2563eb", "700": "#1d4ed8", "800": "#1e40af", "900": "#1e3a8a", "950": "#172554" }
            },
            maxHeight: {
                'table-xl': '60rem',
            }
        },
        fontFamily: {
            'body': [
                'Inter',
                'ui-sans-serif',
                'system-ui',
                '-apple-system',
                'system-ui',
                'Segoe UI',
                'Roboto',
                'Helvetica Neue',
                'Arial',
                'Noto Sans',
                'sans-serif',
                'Apple Color Emoji',
                'Segoe UI Emoji',
                'Segoe UI Symbol',
                'Noto Color Emoji'
            ],
            'sans': [
                'Inter',
                'ui-sans-serif',
                'system-ui',
                '-apple-system',
                'system-ui',
                'Segoe UI',
                'Roboto',
                'Helvetica Neue',
                'Arial',
                'Noto Sans',
                'sans-serif',
                'Apple Color Emoji',
                'Segoe UI Emoji',
                'Segoe UI Symbol',
                'Noto Color Emoji'
            ],
            'mono': [
                'ui-monospace',
                'SFMono-Regular',
                'MesloLGL Nerd Font Mono',
                'Cascadia Mono',
                'Courier New'
            ]
        }
    }
}