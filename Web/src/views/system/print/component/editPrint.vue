<template>
	<div class="sys-print-container">
		<div class="print-dialog">
			<el-dialog v-model="state.isShowDialog" draggable overflow destroy-on-close fullscreen>
				<template #header>
					<div style="color: #fff">
						<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
						<span> {{ props.title }} </span>
					</div>
				</template>
				<div style="margin: 0px; height: 100%;">
					<HiprintDesign :mode-index="mode" ref="hiprintDesignRef" />
				</div>
				<template #footer>
					<span class="dialog-footer" style="margin-top: 10px">
						<el-button @click="cancel">Cancel</el-button>
						<el-button type="primary" @click="submit">Save template</el-button>
					</span>
				</template>
			</el-dialog>
		</div>

		<el-dialog v-model="state.showDialog2" draggable overflow destroy-on-close width="600px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>{{ props.title }}</span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="10">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Template Name" prop="name" :rules="[{ required: true, message: 'Template name cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.name" placeholder="Template Name" clearable />
						</el-form-item>
					</el-col>

					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Sort">
							<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="state">
							<el-radio-group v-model="state.ruleForm.status">
								<el-radio :value="1">enable</el-radio>
								<el-radio :value="2">Disable</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Print Type">
              <g-sys-dict v-model="state.ruleForm.printType" code="PrintTypeEnum" render-as="radio" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Client service address">
							<el-input v-model="state.ruleForm.clientServiceAddress" placeholder="Client service address" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Print parameters">
							<el-input v-model="state.ruleForm.printParam" placeholder="Please enter printing parameters" clearable type="textarea" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Remarks">
							<el-input v-model="state.ruleForm.remark" placeholder="Please enter the remark content" clearable type="textarea" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="templateCancel">Cancel</el-button>
					<el-button type="primary" @click="templateSubmit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditPrint">
import { onMounted, reactive, ref, nextTick } from 'vue';
import HiprintDesign from '/@/views/system/print/component/hiprint/index.vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysPrintApi } from '/@/api-services/api';
import { UpdatePrintInput } from '/@/api-services/models';

const hiprintDesignRef = ref<InstanceType<typeof HiprintDesign>>();
const mode = ref(0);
const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdatePrintInput,
	showDialog2: false,
});

// Page initialization
onMounted(async () => {});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	if (state.ruleForm?.template) {
		let templateJson = JSON.parse(state.ruleForm.template);
		mode.value = templateJson.panels[0].index;
	}
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
	nextTick(() => {
		loadTemplate();
	});
};

// Load template
const loadTemplate = () => {
	hiprintDesignRef.value?.hiprintTemplate.clear();
	hiprintDesignRef.value?.setPrintDataDemo(state.ruleForm.printDataDemo);
	if (JSON.stringify(state.ruleForm) !== '{}') {
		hiprintDesignRef.value?.hiprintTemplate.update(JSON.parse(state.ruleForm.template || '{}'));
		hiprintDesignRef.value?.initPaper();
	}
};

// Cancel
const cancel = () => {
	state.isShowDialog = false;
};

// submit
const submit = async () => {
	state.showDialog2 = true;
	if (state.ruleForm.orderNo == undefined) state.ruleForm.orderNo = 100;
	if (state.ruleForm.status == undefined) state.ruleForm.status = 1;
	if (state.ruleForm.printType == undefined) state.ruleForm.printType = 1;
};

// Template setting canceled
const templateCancel = () => {
	state.showDialog2 = false;
};

// Template settings submitted
const templateSubmit = async () => {
	let templateJson = hiprintDesignRef.value?.hiprintTemplate.getJson();
	templateJson.panels[0].index = hiprintDesignRef.value?.mode;
	state.ruleForm.template = JSON.stringify(templateJson);
	const printDataDemo = hiprintDesignRef.value?.printDataDemo;
	state.ruleForm.printDataDemo = printDataDemo;
	if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
		await getAPI(SysPrintApi).apiSysPrintUpdatePost(state.ruleForm);
	} else {
		await getAPI(SysPrintApi).apiSysPrintAddPost(state.ruleForm);
	}
	cancel();
	templateCancel();
	emits('handleQuery');
};

// Export object
defineExpose({ openDialog });
</script>
