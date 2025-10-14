import { defineConfig } from 'vite';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
    plugins: [
        tailwindcss()
    ],
    root: '.',
    publicDir: 'assets',
    build: {
        manifest: true,
        outDir: '../dist',
        emptyOutDir: true,
        assetsDir: '',
        rollupOptions: {
            input: {
                main: './main.js'
            }
        }
    },
    server: {
        port: 5173,
        strictPort: true,
        host: 'localhost',
        cors: {
            origin: '*',
            credentials: true
        },
        hmr: {
            protocol: 'ws',
            host: 'localhost',
            port: 5173
        },
        proxy: {
            '/api': {
                target: 'http://localhost:5086',
                changeOrigin: true,
                secure: false
            }
        }
    }
});