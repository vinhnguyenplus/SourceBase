import vue from '@vitejs/plugin-vue';
import { resolve } from 'path';
import { defineConfig, loadEnv, ConfigEnv } from 'vite';
import vueSetupExtend from 'vite-plugin-vue-setup-extend';
import compression from 'vite-plugin-compression2';
import { buildConfig } from './src/utils/build';
import vueJsx from '@vitejs/plugin-vue-jsx';
import { CodeInspectorPlugin } from 'code-inspector-plugin';
import fs from 'fs';
import { visualizer } from 'rollup-plugin-visualizer';
import { webUpdateNotice } from '@plugin-web-update-notification/vite';
import vitePluginsAutoI18n, { EmptyTranslator, YoudaoTranslator } from 'vite-auto-i18n-plugin';
const pathResolve = (dir: string) => {
	return resolve(__dirname, '.', dir);
};

const alias: Record<string, string> = {
	'/@': pathResolve('./src/'),
	'vue-i18n': 'vue-i18n/dist/vue-i18n.cjs.js',
	'ezuikit-js': pathResolve('node_modules/ezuikit-js/ezuikit.js'),
};

const viteConfig = defineConfig((mode: ConfigEnv) => {
	const env = loadEnv(mode.mode, process.cwd());
	fs.writeFileSync('./public/config.js', `window.__env__ = ${JSON.stringify(env, null, 2)} `);
	return {
		plugins: [
			visualizer({ open: false }), // Open the visual analysis page
			CodeInspectorPlugin({
				bundler: 'vite',
				hotKeys: ['shiftKey'],
			}),
			vue(),
			vueJsx(),
			webUpdateNotice({
				versionType: 'build_timestamp',
				notificationConfig: {
					placement: 'topLeft',
				},
				notificationProps: {
					title: '📢 System Update',
					description: 'The system has been updated，Please refresh the page！',
					buttonText: 'Refresh',
					dismissButtonText: 'Ignore',
				},
			}),
			vueSetupExtend(),
			compression({
				deleteOriginalAssets: false, // Delete the source file?
				threshold: 5120, // Greater than 5KB File in progress gzip Compress，unitBytes
				skipIfLargerOrEqual: true, // If the compressed file size is equal to or greater than the original file，Then skip compression
				// algorithm: 'gzip', // Compression algorithm，Optional[‘gzip’，‘brotliCompress’，‘deflate’，‘deflateRaw’]
				// exclude: [/\.(br)$/, /\.(gz)$/], // Exclude specified files
			}),
			JSON.parse(env.VITE_OPEN_CDN) ? buildConfig.cdn() : null,
			// Instructions for use https://github.com/auto-i18n/auto-i18n-translation-plugins
			vitePluginsAutoI18n({
				// Whether to trigger translation
				enabled: false,
				originLang: 'vi', //Source Language，Translate based on this language
				targetLangList: ['zh-hk', 'zh-tw', 'en', 'it', 'vi'], // Target language list，Supports configuring multiple languages
				translator: new EmptyTranslator(), // Generate onlyWeb\lang\index.jsonDocument
				// translator: new YoudaoTranslator({ // Youdao Real-time Translation
				// appId: 'The one you applied forappId',
				// appKey: 'The one you applied forappKey'
				// })
			}),
		],
		root: process.cwd(),
		resolve: { alias },
		base: mode.command === 'serve' ? './' : env.VITE_PUBLIC_PATH,
		optimizeDeps: { exclude: ['vue-demi'] },
		server: {
			host: '0.0.0.0',
			port: env.VITE_PORT as unknown as number,
			open: JSON.parse(env.VITE_OPEN),
			hmr: true,
			proxy: {
				'^/[Uu]pload': {
					target: env.VITE_API_URL,
					changeOrigin: true,
				},
			},
		},
		build: {
			outDir: 'dist',
			chunkSizeWarningLimit: 1500,
			assetsInlineLimit: 5000, // Imports or referenced resources smaller than this threshold will be inlined as base64 Encoding
			sourcemap: false, // Whether to generate after building source map Document
			extractComments: false, // Remove comments
			minify: 'terser', // After activation terserOptions The configuration is effective
			terserOptions: {
				compress: {
					drop_console: true, // Remove in production environmentconsole
					drop_debugger: true,
				},
			},
			rollupOptions: {
				output: {
					chunkFileNames: 'assets/js/[name]-[hash].js', // The name of the imported file
					entryFileNames: 'assets/js/[name]-[hash].js', // The name of the package's entry file
					assetFileNames: 'assets/[ext]/[name]-[hash].[ext]', // Resource file like Font，images, etc.
					manualChunks(id) {
						if (id.includes('node_modules')) {
							return id.toString().match(/\/node_modules\/(?!.pnpm)(?<moduleName>[^\/]*)\//)?.groups!.moduleName ?? 'vender';
						}
					},
				},
				...(JSON.parse(env.VITE_OPEN_CDN) ? { external: buildConfig.external } : {}),
			},
		},
		css: { preprocessorOptions: { css: { charset: false }, scss: { silenceDeprecations: ['legacy-js-api', 'global-builtin', 'fs-importer-cwd', 'import'] } } },
		define: {
			__VUE_I18N_LEGACY_API__: JSON.stringify(false),
			__VUE_I18N_FULL_INSTALL__: JSON.stringify(false),
			__INTLIFY_PROD_DEVTOOLS__: JSON.stringify(false),
			__NEXT_VERSION__: JSON.stringify(process.env.npm_package_version),
			__NEXT_NAME__: JSON.stringify(process.env.npm_package_name),
		},
	};
});

export default viteConfig;
