<template>
	<div class="sys-open-access-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="600px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Key /> </el-icon>
					<span> Generate signature </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Identity mark" prop="accessKey">
							<el-input v-model="state.ruleForm.accessKey" placeholder="Identity mark" readonly />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="key" prop="accessSecret">
							<el-input v-model="state.ruleForm.accessSecret" placeholder="key" readonly> </el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="InterfaceRequest address" prop="url">
							<el-input v-model="state.ruleForm.url" placeholder="InterfaceRequest address" class="input-with-select" clearable>
								<template #prepend>
                  <g-sys-dict v-model="state.ruleForm.method" code="HttpMethodEnum" render-as="select" placeholder="Request Method" style="width: 100px" />
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Timestamp" prop="timestamp">
							<el-input v-model="state.ruleForm.timestamp" placeholder="Enter or get timestamp" clearable>
								<template #append>
									<el-button @click="getTimeStamp">Obtain</el-button>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="random number" prop="nonce">
							<el-input v-model="state.ruleForm.nonce" placeholder="Enter or get a random number" clearable>
								<template #append>
									<el-button @click="getNonce">Obtain</el-button>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Signature" prop="sign">
							<el-input v-model="state.sign" placeholder="Automatically generated after filling in the information" readonly> </el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysOpenAccessEdit">
import { reactive, ref, watch } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysOpenAccessApi } from '/@/api-services/api';
import { GenerateSignatureInput, HttpMethodEnum } from '/@/api-services/models';

const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as GenerateSignatureInput,
	sign: '', // generated signature
});

watch([() => state.ruleForm.method, () => state.ruleForm.url, () => state.ruleForm.timestamp, () => state.ruleForm.nonce], () => {
	if (
		state.ruleForm.method == undefined ||
		state.ruleForm.method == null ||
		!state.ruleForm.url ||
		!state.ruleForm.timestamp ||
		!state.ruleForm.nonce ||
		/^\d+$/.test(state.ruleForm.timestamp as unknown as string) == false // timestamp must be numeric
	) {
		state.sign = '';
		return;
	}

	generateSign();
});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = {
		accessKey: row?.accessKey,
		accessSecret: row?.accessSecret,
		method: HttpMethodEnum.NUMBER_0,
		url: '',
	};
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

/** Generate key */
const createSecret = async () => {
	var res = await getAPI(SysOpenAccessApi).apiSysOpenAccessSecretPost();
	state.ruleForm.accessSecret = res.data.result!;
};

/** Get the current timestamp (accurate to seconds) */
const getTimeStamp = () => {
	const timestamp = Math.floor(Date.now() / 1000);
	state.ruleForm.timestamp = timestamp;
};

/** Get random number */
const getNonce = () => {
	var nonce = '';
	for (var i = 0; i < 6; i++) {
		nonce += Math.floor(Math.random() * 10);
	}
	state.ruleForm.nonce = nonce;
};

/** Generate signature */
const generateSign = async () => {
	var res = await getAPI(SysOpenAccessApi).apiSysOpenAccessGenerateSignaturePost(state.ruleForm);
	state.sign = res.data.result!;
};

// Export object
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
:deep(.input-with-select) {
	.el-input-group__prepend {
		background-color: var(--el-fill-color-blank);
	}
}
</style>
