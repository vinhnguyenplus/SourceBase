<script lang="ts" setup>
import { nextTick, onMounted, ref } from "vue";
import QRCode from 'qrcodejs2-fixes';

// Component properties
const props = defineProps({
	realName: String,
	number: String,
	callbackUrl: String,
	callbackData: String,
  needToken: Boolean,
});

// Define variable content
const qrcodeRef = ref<HTMLElement | null>(null);

// Initialize to generate QR code
const initQrcode = () => {
	nextTick(() => {
		const token = props.needToken ? Local.get(accessTokenKey) : '';
		const code = encodeURIComponent(JSON.stringify({
			realName: props.realName,
			number: props.number,
			callbackUrl: props.callbackUrl,
			callbackData: props.callbackData
		}));
		let url = `${location.origin}/$callTel#/$callTel?code=${encodeURIComponent(code)}&token=${encodeURIComponent(token)}`;
		(<HTMLElement>qrcodeRef.value).innerHTML = '';
		new QRCode(qrcodeRef.value, {
			text: url,
			width: 260,
			height: 260,
			colorDark: '#000000',
			colorLight: '#ffffff',
		});
	});
};

// Make a call
const callTel = () => {
	location.href = 'tel:' + props.number;
}

// When the page loads
onMounted(() => {
	initQrcode();
});
</script>

<template>
	<el-popover placement="bottom" width="300" trigger="click">
		<template #reference>
			<i class="iconfont icon-dianhua" v-bind="$attrs" />
		</template>
		<el-descriptions direction="vertical" :column="1" border>
			<el-descriptions-item align="center">
				<template #label>
					<el-button @click="callTel">Dial directly</el-button>
				</template>
			</el-descriptions-item>
			<el-descriptions-item width="140" align="center">
				Scan with your mobile phone
				<template #label>
					<div ref="qrcodeRef" />
				</template>
			</el-descriptions-item>
		</el-descriptions>
	</el-popover>
</template>

<style scoped lang="scss">
.call-bar-container {
	display: flex;
}
</style>