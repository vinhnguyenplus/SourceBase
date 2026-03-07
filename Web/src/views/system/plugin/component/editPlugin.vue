<template>
	<div class="sys-plugin-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="900px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-tabs v-model="state.selectedTabName">
				<el-tab-pane label="Plugin Information">
					<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" style="height: 500px">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Function Name" prop="name" :rules="[{ required: true, message: 'Function name cannot be empty', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.name" placeholder="Function Name" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Assembly Name">
									<el-input v-model="state.ruleForm.assemblyName" placeholder="Assembly Name" clearable />
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
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="Remarks">
									<el-input v-model="state.ruleForm.remark" placeholder="Please enter the remark content" clearable type="textarea" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="C# code">
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

<script lang="ts" setup name="sysEditPlugin">
import { reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';
import * as monaco from 'monaco-editor';

import { getAPI } from '/@/utils/axios-utils';
import { SysPluginApi } from '/@/api-services/api';
import { UpdatePluginInput } from '/@/api-services/models';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const monacoEditorRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdatePluginInput,
	selectedTabName: '0', // selected tab
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
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();

	// Delay the value to prevent failure to obtain it
	setTimeout(() => {
		if (monacoEditor == null) initMonacoEditor();
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

		state.ruleForm.csharpCode = monacoEditor.getValue();
		if (state.ruleForm.csharpCode.length < 100) {
			ElMessage.warning('Please write C# code correctly');
			return;
		}
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysPluginApi).apiSysPluginUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysPluginApi).apiSysPluginAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
