<template>
	<el-tooltip :visible="state.capsLockVisible" effect="light" content="Caps Lock is on" placement="top">
		<el-form ref="ruleFormRef" :model="state.ruleForm" size="large" :rules="state.rules" class="login-content-form">
			<el-form-item class="login-animation1" prop="account">
				<el-input ref="accountRef" text placeholder="Please enter account number" v-model="state.ruleForm.account" clearable autocomplete="off" @keyup.enter.native="handleSignIn">
					<template #prefix>
						<el-icon>
							<ele-User />
						</el-icon>
					</template>
				</el-input>
			</el-form-item>
			<el-form-item class="login-animation2" prop="password">
				<el-input ref="passwordRef" :type="state.isShowPassword ? 'text' : 'password'" placeholder="Please enter password" v-model="state.ruleForm.password" autocomplete="off" @keyup.enter.native="handleSignIn">
					<template #prefix>
						<el-icon>
							<ele-Unlock />
						</el-icon>
					</template>
					<template #suffix>
						<i class="iconfont el-input__icon login-content-password" :class="state.isShowPassword ? 'icon-yincangmima' : 'icon-xianshimima'" @click="state.isShowPassword = !state.isShowPassword">
						</i>
					</template>
				</el-input>
			</el-form-item>
			<el-form-item class="login-animation2" prop="tenantId" clearable v-if="!props.tenantInfo?.id && !state.hideTenantForLogin">
				<el-select v-model="state.ruleForm.tenantId" placeholder="Please select a tenant" style="width: 100%" filterable>
					<template #prefix>
						<i class="iconfont icon-shuxingtu el-input__icon"></i>
					</template>
					<el-option :value="item.value" :label="item.label" v-for="(item, index) in tenantInfo?.list" :key="index" />
				</el-select>
			</el-form-item>
			<el-form-item class="login-animation3" prop="captcha" v-if="state.captchaEnabled">
				<el-col :span="15">
					<el-input ref="codeRef" text maxlength="4" placeholder="Please enter the verification code" v-model="state.ruleForm.code" clearable autocomplete="off" @keyup.enter.native="handleSignIn">
						<template #prefix>
							<el-icon>
								<ele-Position />
							</el-icon>
						</template>
					</el-input>
				</el-col>
				<el-col :span="1"></el-col>
				<el-col :span="8">
					<div :class="[state.expirySeconds > 0 ? 'login-content-code' : 'login-content-code-expired']" @click="getCaptcha">
						<img class="login-content-code-img" width="130px" height="38px" :src="state.captchaImage" style="cursor: pointer" />
					</div>
				</el-col>
			</el-form-item>
			<el-form-item class="login-animation4">
				<el-button type="primary" class="login-content-submit" round v-waves @click="handleSignIn" :loading="state.loading.signIn">
					<span>Log in</span>
				</el-button>
			</el-form-item>
			<!-- <div class="font12 mt30 login-animation4 login-msg">* Warm reminder: It is recommended to use Google, Microsoft Edge, version 79.0.1072.62 and above browsers, please use the fast mode for 360 browsers</div> -->
			<!-- <el-button type="primary" round v-waves @click="weixinSignIn" :loading="state.loading.signIn"></el-button> -->
		</el-form>
	</el-tooltip>
	<div class="dialog-header">
		<el-dialog v-model="state.rotateVerifyVisible" :show-close="false">
			<DragVerifyImgRotate
				ref="dragRef"
				:imgsrc="state.rotateVerifyImg"
				v-model:isPassing="state.isPassRotate"
				text="Please hold down the slider and drag"
				successText="Verification passed"
				handlerIcon="fa fa-angle-double-right"
				successIcon="fa fa-hand-peace-o"
				@passcallback="passRotateVerify"
			/>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="loginAccount">
import { reactive, computed, ref, onMounted, defineAsyncComponent, onUnmounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage, InputInstance } from 'element-plus';
import { initBackEndControlRoutes } from '/@/router/backEnd';
import { Local, Session } from '/@/utils/storage';
import { formatAxis } from '/@/utils/formatTime';
import { NextLoading } from '/@/utils/loading';
import { sm2 } from 'sm-crypto-v2';
import { useThemeConfig } from '/@/stores/themeConfig';
import { storeToRefs } from 'pinia';

import { accessTokenKey, clearTokens, feature, getAPI } from '/@/utils/axios-utils';
import { SysAuthApi } from '/@/api-services/api';

const props = defineProps({
	tenantInfo: {
		required: true,
		type: Object,
	},
});

// Rotate picture slider component
// import verifyImg from '/@/assets/logo-mini.svg';
const DragVerifyImgRotate = defineAsyncComponent(() => import('/@/components/dragVerify/dragVerifyImgRotate.vue'));

const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);

const route = useRoute();
const router = useRouter();

const ruleFormRef = ref();
const accountRef = ref<InputInstance>();
const passwordRef = ref<InputInstance>();
const codeRef = ref<InputInstance>();

const dragRef: any = ref(null);
const state = reactive({
	isShowPassword: false,
	ruleForm: {
		account: window.__env__.VITE_DEFAULT_USER,
		password: window.__env__.VITE_DEFAULT_USER_PASSWORD,
		tenantId: props.tenantInfo?.id ?? undefined,
		code: '',
		codeId: 0,
	},
	rules: {
		account: [{ required: true, message: 'Please enter account number', trigger: 'blur' }],
		password: [{ required: true, message: 'Please enter password', trigger: 'blur' }],
		// code: [{ required: true, message: 'Please enter the verification code', trigger: 'blur' }],
	},
	loading: {
		signIn: false,
	},
	captchaImage: '',
	rotateVerifyVisible: false,
	// rotateVerifyImg: verifyImg,
	rotateVerifyImg: themeConfig.value.logoUrl,
	secondVerEnabled: false,
	captchaEnabled: false,
	isPassRotate: false,
	capsLockVisible: false,
	hideTenantForLogin: false,
	expirySeconds: 60, // Verification code expiration time
});

// Verification code expiration timer
let timer: any = null;

// Page initialization
onMounted(async () => {
	// If the URL contains Token parameters (third-party login)
	const accessToken = route.query.token;
	if (accessToken) await saveTokenAndInitRoutes(accessToken);
	watch(
		() => themeConfig.value.isLoaded,
		(isLoaded) => {
			if (isLoaded) {
				// Get login configuration
				state.hideTenantForLogin = themeConfig.value.hideTenantForLogin ?? true;
				state.secondVerEnabled = themeConfig.value.secondVer ?? true;
				state.captchaEnabled = themeConfig.value.captcha ?? true;

				// Get verification code
				getCaptcha();

				// Registration verification code expiration timer
				if (state.captchaEnabled) {
					timer = setInterval(() => {
						if (state.expirySeconds > 0) state.expirySeconds -= 1;
					}, 1000);
				}
			}
		},
		{ immediate: true }
	);

	// Detect uppercase and lowercase keys/CapsLK
	document.addEventListener('keyup', handleKeyPress);
});

// Page unloading
onUnmounted(() => {
	// Destroy verification code expiration timer
	clearInterval(timer);
	timer = null;

	document.removeEventListener('keyup', handleKeyPress);
});

// Detect uppercase and lowercase keys
const handleKeyPress = (e: KeyboardEvent) => {
	if (e.getModifierState != undefined) state.capsLockVisible = e.getModifierState('CapsLock');
};

// Get verification code
const getCaptcha = async () => {
	if (!state.captchaEnabled) return;

	state.ruleForm.code = '';
	const res = await getAPI(SysAuthApi)
		.apiSysAuthCaptchaGet()
		.then((res) => res.data.result);
	state.captchaImage = 'data:text/html;base64,' + res?.img;
	state.expirySeconds = res?.expirySeconds;
	state.ruleForm.codeId = res?.id;
};

// Get time
const currentTime = computed(() => {
	return formatAxis(new Date());
});

// Log in
const onSignIn = async () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return false;

		try {
			state.loading.signIn = true;

			// SM2 encryption password
			// const keys = SM2.generateKeyPair();
			const publicKey = window.__env__.VITE_SM_PUBLIC_KEY;
			const password = sm2.doEncrypt(state.ruleForm.password, publicKey, 1);

			state.ruleForm.tenantId ??= props.tenantInfo.id ?? props.tenantInfo.list[0]?.value ?? undefined;
			// console.log(state.ruleForm.tenantId);
			const [err, res] = await feature(getAPI(SysAuthApi).apiSysAuthLoginPost({ ...state.ruleForm, password: password } as any));
			if (err) {
				getCaptcha(); // Get verification code again
				return;
			}
			if (res.data.result?.accessToken == undefined) {
				getCaptcha(); // Get verification code again
				ElMessage.error('LoginFailure，Please checkAccount number！');
				return;
			}
			// Record user-defined homepage settings
			Session.set('homepage', res.data.result?.homepage);
			await saveTokenAndInitRoutes(res.data.result?.accessToken);
		} finally {
			state.loading.signIn = false;
		}
	});
};

// Keep Token and initialize routing
const saveTokenAndInitRoutes = async (accessToken: string | any) => {
	// Cache token
	Local.set(accessTokenKey, accessToken);
	// Local.set(refreshAccessTokenKey, refreshAccessToken);
	Session.set('token', accessToken);

	// After adding the dynamic route, perform router jump, otherwise an error may be reported No match found for location with path "/"
	const isNoPower = await initBackEndControlRoutes();
	signInSuccess(isNoPower); // Execute signInSuccess again
};

// Jump after successful login
const signInSuccess = (isNoPower: boolean | undefined) => {
	if (isNoPower) {
		ElMessage.warning('Sorry, you do not have login permission');
		clearTokens(); // Clear Token Cache
	} else {
		// Initialization login success time greeting
		let currentTimeInfo = currentTime.value;
		// If the login is successful, jump to the home page. If the path is copied and pasted and is not the home page/login page, then after successful login, you will be redirected to the corresponding path.
		if (route.query?.redirect) {
			router.push({
				path: <string>route.query?.redirect,
				query: Object.keys(<string>route.query?.params).length > 0 ? JSON.parse(<string>route.query?.params) : '',
			});
		} else {
			router.push('/');
		}

		// Login success prompt
		const signInText = 'Welcome back!';
		ElMessage.success(`${currentTimeInfo}，${signInText}`);
		// Add loading to prevent a brief blank when entering the interface for the first time
		NextLoading.start();
	}
};

// Turn on rotation verification
const openRotateVerify = () => {
	state.rotateVerifyVisible = true;
	state.isPassRotate = false;
	dragRef.value?.reset();
};

// Verified by rotation
const passRotateVerify = () => {
	state.rotateVerifyVisible = false;
	state.isPassRotate = true;
	onSignIn();
};

// Login processing
const handleSignIn = () => {
	if (!state.ruleForm.account) {
		accountRef.value?.focus();
	} else if (!state.ruleForm.password) {
		passwordRef.value?.focus();
	} else if (state.captchaEnabled && !state.ruleForm.code) {
		codeRef.value?.focus();
	} else {
		state.secondVerEnabled ? openRotateVerify() : onSignIn();
	}
};

// Export object
defineExpose({ saveTokenAndInitRoutes });
</script>

<style lang="scss" scoped>
.dialog-header {
	:deep(.el-dialog) {
		width: unset !important;

		.el-dialog__header {
			display: none;
		}

		.el-dialog__wrapper {
			position: absolute !important;
		}

		.v-modal {
			position: absolute !important;
		}
	}
}

.login-content-form {
	margin-top: 20px;

	@for $i from 0 through 4 {
		.login-animation#{$i} {
			opacity: 0;
			animation-name: error-num;
			animation-duration: 0.5s;
			animation-fill-mode: forwards;
			animation-delay: calc($i/10) + s;
		}
	}

	.login-content-password {
		display: inline-block;
		width: 20px;
		cursor: pointer;

		&:hover {
			color: #909399;
		}
	}

	.login-content-code {
		display: flex;
		align-items: center;
		justify-content: space-around;
		position: relative;

		.login-content-code-img {
			width: 100%;
			height: 40px;
			line-height: 40px;
			background-color: #ffffff;
			border: 1px solid rgb(220, 223, 230);
			cursor: pointer;
			transition: all ease 0.2s;
			border-radius: 4px;
			user-select: none;

			&:hover {
				border-color: #c0c4cc;
				transition: all ease 0.2s;
			}
		}
	}

	.login-content-code-expired {
		@extend .login-content-code;
		&::before {
			content: 'The verification code has expired';
			position: absolute;
			top: 0;
			left: 0;
			right: 0;
			bottom: 0;
			border-radius: 4px;
			background-color: rgba(0, 0, 0, 0.5);
			color: #ffffff;
			text-align: center;
		}
	}

	.login-content-submit {
		width: 100%;
		letter-spacing: 2px;
		font-weight: 300;
		margin-top: 15px;
	}

	.login-msg {
		color: var(--el-text-color-placeholder);
	}
}
</style>
