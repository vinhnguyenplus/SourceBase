<template>
	<div class="sys-codeGenPreview-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="70%">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<div :class="[state.current?.endsWith('.cs') ? 'cs-style' : state.current?.endsWith('.vue') ? 'vue-style' : 'js-style']">
				<el-segmented v-model="state.current" :options="state.options" block @change="handleChange">
					<template #default="{ item }">
						<div class="pd4">
							<SvgIcon :name="item.icon" class="mb4" />
							<div>{{ item.value }}</div>
						</div>
					</template>
				</el-segmented>
			</div>
			<div ref="monacoEditorRef" v-loading="state.loading" class="code-container"></div>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-Close" @click="cancel">closure</el-button>
					<el-button icon="ele-CopyDocument" type="primary" @click="handleCopy">copy</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysPreviewCode">
import { reactive, ref, nextTick, toRaw } from 'vue';
import * as monaco from 'monaco-editor';
import EditorWorker from 'monaco-editor/esm/vs/editor/editor.worker?worker';
import commonFunction from '/@/utils/commonFunction';

import { getAPI } from '/@/utils/axios-utils';
import { SysCodeGenApi } from '/@/api-services/api';

const { copyText } = commonFunction();

const props = defineProps({
	title: String,
});
const monacoEditorRef = ref();
const state = reactive({
	isShowDialog: false,
	options: [] as any, // Segmenter options
	current: '', // selected segment
	codes: [] as any, // Preview code
  loading: true
});

// Prevent monaco from reporting porn
self.MonacoEnvironment = {
	getWorker: (_: string, label: string) => new EditorWorker(),
};

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
const openDialog = async (row: any) => {
  state.loading = true;
  try {
    state.isShowDialog = true;
    const { data } = await getAPI(SysCodeGenApi).apiSysCodeGenPreviewPost(row);
    state.codes = data.result ?? [];
    state.options = Object.keys(data.result ?? []).map((fileName: string) => ({
      value: fileName,
      icon: fileName?.endsWith('.cs') ? 'fa fa-hashtag' : fileName?.endsWith('.vue') ? 'fa fa-vimeo' : 'fa fa-file-code-o',
    }));
    state.current = state.options?.[0]?.value ?? '';
  }  catch (e) { /* empty */ }
  state.loading = false;
	if (monacoEditor == null) initMonacoEditor();
	// Prevent not being able to get it
	nextTick(() => {
		monacoEditor.setValue(state.codes[state.current]);
	});
};

// Switch code when segmenter changes
const handleChange = (current: any) => {
	monacoEditor.setValue(state.codes[current]);
};

// Cancel
const cancel = () => {
	state.isShowDialog = false;
};

//Copy code
const handleCopy = () => {
	copyText(state.codes[state.current]);
};

// Export object
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
.cs-style .el-segmented {
	--el-segmented-item-selected-bg-color: #5c2d91;
}
.vue-style .el-segmented {
	--el-segmented-item-selected-bg-color: #42b883;
}
.js-style .el-segmented {
	--el-segmented-item-selected-bg-color: #e44d26;
}
:deep(.el-dialog__body) {
	height: calc(100vh - 160px) !important;

    .code-container {
        width: 100%;
        height: calc(100% - 48px);
    }
}
</style>
