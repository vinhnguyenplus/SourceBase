<template>
	<div class="login-scan-container">
		<el-select v-model="state.tenantId" v-if="!props.tenantInfo.id && !themeConfig.hideTenantForLogin" placeholder="Please select a tenant" clearable class="mb30" style="width: 260px; align-self: center;" filterable>
			<template #prefix>
				<i class="iconfont icon-shuxingtu el-input__icon"></i>
			</template>
			<el-option :value="item.value" :label="item.label" v-for="(item, index) in tenantInfo.list" :key="index" />
		</el-select>
		<div ref="qrcodeRef"></div>
		<div class="font12 mt20 login-msg">
			<i class="iconfont icon-saoyisao mr5"></i>
			<span>Open your phone to scan and quickly log in/register</span>
		</div>
	</div>
</template>

<script setup lang="ts" name="loginScan">
import {ref, onMounted, nextTick, reactive} from 'vue';
import QRCode from 'qrcodejs2-fixes';
import {useThemeConfig} from '/@/stores/themeConfig';
import {storeToRefs} from 'pinia';

const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const props = defineProps({
	tenantInfo: {
		required: true,
		type: Object,
	},
});

const state = reactive({
	tenantId: props.tenantInfo.id,
});

// Define variable content
const qrcodeRef = ref<HTMLElement | null>(null);

// Initialize to generate QR code
const initQrcode = () => {
	nextTick(() => {
		(<HTMLElement>qrcodeRef.value).innerHTML = '';
		new QRCode(qrcodeRef.value, {
			text: `https://gitee.com/zuohuaijun/Admin.NET`,
			width: 260,
			height: 260,
			colorDark: '#000000',
			colorLight: '#ffffff',
		});
	});
};
// When the page loads
onMounted(() => {
	initQrcode();
});
</script>

<style scoped lang="scss">
.login-scan-animation {
	opacity: 0;
	animation-name: error-num;
	animation-duration: 0.5s;
	animation-fill-mode: forwards;
}
.login-scan-container {
	padding: 0 20px 20px;
	display: flex;
	flex-direction: column;
	text-align: center;
	@extend .login-scan-animation;
	animation-delay: 0.1s;
	:deep(img) {
		margin: auto;
        border: 1px solid var(--el-border-color);
        padding: 10px;
	}
	.login-msg {
		display: flex;
		align-items: center;
		justify-content: center;
		color: var(--el-text-color-placeholder);
		@extend .login-scan-animation;
		animation-delay: 0.2s;
	}
}
</style>
