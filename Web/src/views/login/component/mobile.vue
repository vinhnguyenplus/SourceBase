<template>
	<el-form size="large" class="login-content-form">
		<el-form-item class="login-animation1" v-if="!props.tenantInfo.id && !themeConfig.hideTenantForLogin">
			<el-select v-model="state.ruleForm.tenantId" placeholder="Please select a tenant" clearable style="width: 100%" filterable>
				<template #prefix>
					<i class="iconfont icon-shuxingtu el-input__icon"></i>
				</template>
				<el-option :value="item.value" :label="item.label" v-for="(item, index) in tenantInfo.list" :key="index" />
			</el-select>
		</el-form-item>
		<el-form-item class="login-animation1">
			<el-input text placeholder="Please enter your phone number" v-model="state.ruleForm.phone" clearable autocomplete="off">
				<template #prefix>
					<i class="iconfont icon-dianhua el-input__icon"></i>
				</template>
			</el-input>
		</el-form-item>
		<el-form-item class="login-animation2">
			<el-col :span="15">
				<el-input text maxlength="6" placeholder="Please enter the verification code" v-model="state.ruleForm.code" clearable autocomplete="off">
					<template #prefix>
						<el-icon class="el-input__icon"><ele-Position /></el-icon>
					</template>
				</el-input>
			</el-col>
			<el-col :span="1"></el-col>
			<el-col :span="8">
				<el-button v-waves class="login-content-code" :loading="state.loading" :disabled="state.disabled" @click="getSmsCode">
					{{ state.btnText }}
				</el-button>
			</el-col>
		</el-form-item>
		<el-form-item class="login-animation3">
			<el-button round type="primary" v-waves class="login-content-submit" @click="onSignIn">
				<span>Log in</span>
			</el-button>
		</el-form-item>
		<div class="font12 mt30 login-animation4 login-msg">* Warm reminder: It is recommended to use Google, Microsoft Edge, version 79.0.1072.62 and above browsers, please use the fast mode for 360 browsers</div>
	</el-form>
</template>

<script setup lang="ts" name="loginMobile">
import { reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { verifyPhone } from '/@/utils/toolsValidate';
import { getAPI } from '/@/utils/axios-utils';
import { SysSmsApi, SysAuthApi } from '/@/api-services/api';
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';

const props = defineProps({
	tenantInfo: {
		required: true,
		type: Object,
	},
});
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const state = reactive({
	ruleForm: {
		tenantId: props.tenantInfo.id,
		phone: '',
		code: '',
	},
	btnText: 'ObtainVerification Code',
	loading: false,
	disabled: false,
	timer: null as any,
});

// Get SMS verification code
const getSmsCode = async () => {
	state.ruleForm.code = '';
	if (!verifyPhone(state.ruleForm.phone)) {
		ElMessage.error('Please enter the phone number correctly!');
		return;
	}

	await getAPI(SysSmsApi).apiSysSmsSendSmsPhoneNumberTemplateIdPost(state.ruleForm.phone, '0');

	// Clicking is prohibited during the countdown
	state.disabled = true;

	// clear timer
	state.timer && clearInterval(state.timer);

	// Start timer
	var duration = 60;
	state.timer = setInterval(() => {
		duration--;
		state.btnText = `${duration} secondafter againObtain`;
		if (duration <= 0) {
			state.btnText = 'ObtainVerification Code';
			state.disabled = false; // The restore button is clickable
			clearInterval(state.timer); // clear timer
		}
	}, 1000);
};

// Log in
const onSignIn = async () => {
	state.ruleForm.tenantId ??= props.tenantInfo.id ?? props.tenantInfo.list[0]?.value ?? undefined;
	const res = await getAPI(SysAuthApi).apiSysAuthLoginPhonePost(state.ruleForm);
	if (res.data.result?.accessToken == undefined) {
		ElMessage.error('LoginFailure，Please checkAccount number！');
		return;
	}

	// // System login
	// await accountRef.value?.saveTokenAndInitRoutes(res.data.result?.accessToken);
};
</script>

<style scoped lang="scss">
.login-content-form {
	margin-top: 20px;
	@for $i from 1 through 4 {
		.login-animation#{$i} {
			opacity: 0;
			animation-name: error-num;
			animation-duration: 0.5s;
			animation-fill-mode: forwards;
			animation-delay: calc($i/10) + s;
		}
	}
	.login-content-code {
		width: 100%;
		padding: 0;
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
