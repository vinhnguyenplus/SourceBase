<template>
	<div class="sys-jobDetail-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="900px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-tabs v-model="state.selectedTabName">
				<el-tab-pane label="Job information">
					<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" style="height: 500px">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Assignment Number" prop="jobId" :rules="[{ required: true, message: 'Assignment number cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.jobId" placeholder="Assignment Number" :disabled="isEdit" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Group Name" prop="groupName" :rules="[{ required: true, message: 'Group name cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.groupName" placeholder="Group Name" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Create type">
                  <g-sys-dict v-model="state.ruleForm.createType" code="JobCreateTypeEnum" render-as="radio" :disabled="isEdit" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="Execution method">
									<el-radio-group v-model="state.ruleForm.concurrent">
										<el-radio :value="true">Parallel</el-radio>
										<el-radio :value="false">serial</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" v-show="!isEdit && !isHttpCreateType">
								<el-form-item prop="includeAnnotation">
									<template v-slot:label>
										<div>
											Scan trigger
											<el-tooltip raw-content content="thisParameteronly atAdd NewHomeworktimeTake effect<br/>Scan triggers defined on jobs" placement="top">
												<SvgIcon name="fa fa-question-circle-o" :size="15" style="vertical-align: middle" />
											</el-tooltip>
										</div>
									</template>
									<el-radio-group v-model="state.ruleForm.includeAnnotation">
										<el-radio :value="true">Yes</el-radio>
										<el-radio :value="false">no</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Description information">
									<el-input v-model="state.ruleForm.description" placeholder="Description information" clearable type="textarea" :autosize="{ minRows: 1, maxRows: 3 }" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" v-if="!isHttpCreateType">
								<el-form-item label="Extra data">
									<el-input v-model="state.ruleForm.properties" placeholder="Extra data" clearable type="textarea" :autosize="{ minRows: 3, maxRows: 6 }" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" v-if="isHttpCreateType">
								<el-form-item label="Request address">
									<el-input v-model="state.httpJobMessage.requestUri" placeholder="Request address" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" v-if="isHttpCreateType">
								<el-form-item label="Request Method">
									<el-radio-group v-model="state.httpJobMessage.httpMethod">
										<el-radio :value="httpMethodDef.get">Get</el-radio>
										<el-radio :value="httpMethodDef.post">Post</el-radio>
										<el-radio :value="httpMethodDef.put">Put</el-radio>
										<el-radio :value="httpMethodDef.delete">Delete</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" v-if="isHttpCreateType">
								<el-form-item label="Request message body">
									<el-input v-model="state.httpJobMessage.body" placeholder="Request message body" clearable type="textarea" :autosize="{ minRows: 3, maxRows: 6 }" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="script code" :disabled="!isScriptCreateType">
					<div ref="monacoEditorRef" style="width: 100%; height: 500px"></div>
				</el-tab-pane>
			</el-tabs>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditJobDetail">
import { reactive, ref, computed } from 'vue';
import * as monaco from 'monaco-editor';
import { JobScriptCode } from './JobScriptCode';
import { getAPI } from '/@/utils/axios-utils';
import { SysJobApi } from '/@/api-services/api';
import { JobCreateTypeEnum, UpdateJobDetailInput } from '/@/api-services/models';

// HttpMethod definition, serialization of source backend HttpMethod objects
// [Do not] add spaces to the content defined below, otherwise it will not match after JSON.stringify(httpJobMessageNet.HttpMethod) in getHttpJobMessage
const httpMethodDef: Record<string,string> = {
	get: '{"Method":"GET"}',
	post: '{"Method":"POST"}',
	put: '{"Method":"PUT"}',
	delete: '{"Method":"DELETE"}',
};

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const monacoEditorRef = ref();
const state = reactive({
	isShowDialog: false,
	selectedTabName: '0', // Selected tab page
	ruleForm: {} as UpdateJobDetailInput,
	httpJobMessage: { requestUri: '', httpMethod: httpMethodDef.get, body: '' } as HttpJobMessage,
});

// Whether to edit status
const isEdit = computed(() => {
	return state.ruleForm.id != undefined && state.ruleForm.id > 0;
});

// Whether script creation type
const isScriptCreateType = computed(() => {
	return state.ruleForm.createType === JobCreateTypeEnum.NUMBER_1;
});

// Whether Http request creation type
const isHttpCreateType = computed(() => {
	return state.ruleForm.createType === JobCreateTypeEnum.NUMBER_2;
});

// Initialize the monacoEditor object
var monacoEditor: any = null;
const initMonacoEditor = () => {
	monacoEditor = monaco.editor.create(monacoEditorRef.value, {
		theme: 'vs-dark', // theme vs vs-dark hc-black
		value: '', // The value displayed by default
		language: 'csharp',
		formatOnPaste: true,
		wordWrap: 'on', // Automatically wrap lines, pay attention to capitalization
		wrappingIndent: 'indent',
		folding: true, // Whether to fold
		foldingHighlight: true, // Collapse contours
		foldingStrategy: 'indentation', // Folding mode auto | indentation
		showFoldingControls: 'always', // Whether to always display the fold always | mouSEOver
		disableLayerHinting: true, // Equal width optimization
		emptySelectionClipboard: false, // Empty selection clipboard
		selectionClipboard: false, // Select clipboard
		automaticLayout: true, // autolayout
		codeLens: false, // code lens
		scrollBeyondLastLine: false, // Scroll one more screen after scrolling the last line
		colorDecorators: true, // color decorator
		accessibilitySupport: 'auto', // Accessibility support "auto" | "off" | "on"
		lineNumbers: 'on', // Line number Values: "on" | "off" | "relative" | "interval" | function
		lineNumbersMinChars: 5, // Minimum characters for line number number
		//enableSplitViewResizing: false,
		readOnly: false, // Whether to read only the value true | false
	});
};

// Open pop-up window
const openDialog = (row: any) => {
	state.selectedTabName = '0'; // Reset to first tab page
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();

	// HTTP request
	if (row.id && state.ruleForm.createType === JobCreateTypeEnum.NUMBER_2) {
		state.httpJobMessage = getHttpJobMessage(state.ruleForm.properties);
	}

	// Delay the value to prevent failure to obtain it
	setTimeout(() => {
		if (monacoEditor == null) initMonacoEditor();
		monacoEditor.setValue(row.id == undefined ? JobScriptCode : state.ruleForm.scriptCode);
	}, 1);
};

// Close pop-up window
const closeDialog = () => {
	emits('handleQuery');
	state.isShowDialog = false;
};

// Cancel
const cancel = () => {
	state.isShowDialog = false;
};

// submit
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;

		// Script creation type
		if (state.ruleForm.createType === JobCreateTypeEnum.NUMBER_1) {
			state.ruleForm.scriptCode = monacoEditor.getValue();
		} else {
			state.ruleForm.scriptCode = '';
		}

		// Http request creation type
		if (state.ruleForm.createType === JobCreateTypeEnum.NUMBER_2) {
			// Re-encapsulate httpJobMessage and follow the back-end HttpJob serialization requirements. The fields must start with capital letters.
			// HttpJob convention reads the value of the attribute "HttpJob"
			const httpJobPropValue = JSON.stringify({
				RequestUri: state.httpJobMessage.requestUri,
				HttpMethod: JSON.parse(state.httpJobMessage.httpMethod + ''),
				Body: state.httpJobMessage.body,
				ClientName: 'HttpJob',
				EnsureSuccessStatusCode: true,
			});
			const prop = { HttpJob: httpJobPropValue };
			state.ruleForm.properties = JSON.stringify(prop);
		}

		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysJobApi).apiSysJobUpdateJobDetailPost(state.ruleForm);
		} else {
			await getAPI(SysJobApi).apiSysJobAddJobDetailPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Get HttpJobMessage based on task properties
const getHttpJobMessage = (properties: string | undefined | null): HttpJobMessage => {
	if (properties === undefined || properties === null || properties === '') return {};

	const propData = JSON.parse(properties);
	const httpJobMessageNet = JSON.parse(propData['HttpJob']); // HttpJobMessage with backend capitalized

	return {
		requestUri: httpJobMessageNet.RequestUri,
		httpMethod: JSON.stringify(httpJobMessageNet.HttpMethod),
		body: httpJobMessageNet.Body,
	};
};

// Export object
defineExpose({ httpMethodDef, openDialog, getHttpJobMessage });
</script>
