// https://nuxt.com/docs/api/configuration/nuxt-config

export default defineNuxtConfig({

	app: {
		head: {
			htmlAttrs: {
				lang: 'en'
			},
			meta: [
				{'http-equiv': "Content-Type", content:"text/html", charset: "utf-8"},
				{charset: "utf-8"},
				{'http-equiv': "X-UA-Compatible", content: "IE=edge"},
				{name: "verify-v1", content: "SB0VI0B4WbSsa5Ak42B8zbdemFIWxElj6nViTvd0/Cg="},
				{name: "agd-partner-manual-verification"},
				{name: "robots", content: "index, follow"},
			]
		}
	},
	ssr: true,
	css: [
		'~/assets/scss/main.scss',
	],
	vite: {
		css: {
			preprocessorOptions: {
				scss: {
					additionalData: `@use "~/assets/scss/_breakpoint" as *;`
				}
			}
		},

	},
	modules: [
		'@pinia/nuxt',
		'@nuxt/image',
		'nuxt-svg-icon-sprite',
	],
	components: [
		{
			path: '~/components',
			pathPrefix: false,
			global: true
		},
	],
	compatibilityDate: '2024-11-01',
	devtools: { enabled: true },
	nitro: {
		compressPublicAssets: true,
		serveStatic: true,

		routeRules: {
			'/images/**': {
				headers: {
					'cache-control': 'public, max-age=31536000, immutable'
				}
			},
			'/**/*.{js,css,svg,png,jpg,jpeg,gif,webp,woff,woff2,ttf,eot}': {
				headers: {
					'cache-control': 'public, max-age=31536000, immutable'
				}
			},
			'/_nuxt/**': {
				headers: {
					'cache-control': 'public, max-age=31536000, immutable'
				}
			}
		}
	},
	svgIconSprite: {
		sprites: {
			default: {
				importPatterns: [
					'~/assets/svg/**/*.svg'
				]
			}
		}
	},
	runtimeConfig: {
		public: {
			apiBaseUrl: 'http://localhost:5153',
		},
	},
})
