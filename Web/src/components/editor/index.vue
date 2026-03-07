<template>
	<div class="editor-container">
		<Toolbar :editor="editorRef" :mode="mode" />
		<Editor :mode="mode" :defaultConfig="state.editorConfig" :style="{ height }" v-model="state.editorVal" @onCreated="handleCreated" @onChange="handleChange" />
	</div>
</template>

<script setup lang="ts" name="wngEditor">
// https://www.wangeditor.com/v5/for-frame.html#vue3
import '@wangeditor/editor/dist/css/style.css';
import { reactive, shallowRef, watch, onBeforeUnmount } from 'vue';
import { IDomEditor } from '@wangeditor/editor';
import { Toolbar, Editor } from '@wangeditor/editor-for-vue';
import { ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { SysFileApi } from '/@/api-services/api';

// Define the value passed by the parent component
const props = defineProps({
	// Whether to disable
	disable: {
		type: Boolean,
		default: () => false,
	},
	// Content box default placeholder
	placeholder: {
		type: String,
		default: () => 'Please entercontent...',
	},
	// https://www.wangeditor.com/v5/getting-started.html#mode-%E6%A8%A1%E5%BC%8F
	// Mode, optional <default|simple>, default default
	mode: {
		type: String,
		default: () => 'default',
	},
	// high
	height: {
		type: String,
		default: () => '310px',
	},
	// Two-way binding, used to obtain editor.getHtml()
	getHtml: String,
	// Two-way binding, used to obtain editor.getText()
	getText: String,
});

// Define child components to pass values/events to parent components
const emit = defineEmits(['update:getHtml', 'update:getText']);

// Define variable content
const editorRef = shallowRef();
const state = reactive({
	editorConfig: {
		placeholder: props.placeholder,
		// Menu configuration
		MENU_CONF: {
			uploadImage: {
				fieldName: 'file',
				customUpload(file: File, insertFn: any) {
					getAPI(SysFileApi).apiSysFileUploadFilePostForm(file).then(({data}) => {
						if (data.type == 'success' && data.result) {
							editorRef.value.insertNode({ type: 'image', src: data.result.url, alt: data.result.fileName, href: data.result.url, children: [{ text: '' }] })
						} else {
							ElMessage.error('Upload failed!')
						}
					})
				},
			},
			insertImage: {
				checkImage(src: string, alt: string, href: string): boolean | string | undefined {
					if (src.indexOf('http') !== 0) {
						return 'Image URL must start with http/https';
					}
					return true;
				},
			},
		},
	},
	editorVal: props.getHtml,
});

// Editor callback function
const handleCreated = (editor: IDomEditor) => {
	editorRef.value = editor;
};
// When the editor content changes
const handleChange = (editor: IDomEditor) => {
	emit('update:getHtml', editor.getHtml());
	emit('update:getText', editor.getText());
};
// When the page is destroyed
onBeforeUnmount(() => {
	const editor = editorRef.value;
	if (editor == null) return;
	editor.destroy();
});
// Listen for disable changes
// https://gitee.com/lyt-top/vue-next-admin/issues/I4LM7I
watch(
	() => props.disable,
	(bool) => {
		const editor = editorRef.value;
		if (editor == null) return;
		bool ? editor.disable() : editor.enable();
	},
	{
		deep: true,
	}
);
// Monitor changes in two-way binding values ​​for echoing
watch(
	() => props.getHtml,
	(val) => {
		state.editorVal = val;
	},
	{
		deep: true,
	}
);

// expose editorRef
defineExpose({
	ref: editorRef,
});
</script>
<style lang="less">
.editor-container {
	overflow-y: hidden;
	.w-e-bar-item {
		.w-e-select-list {
			height: 150px;
			z-index: 10 !important;
		}
	}
	.w-e-text-container {
		// Lower the level inside the text box
		//z-index: 3 !important;
	}
	.w-e-toolbar {
		// Wrap toolbar
		flex-wrap: wrap;
		z-index: 4 !important;
	}
	.w-e-menu {
		// The most important code
		z-index: auto !important;
		.w-e-droplist {
			// Increase the display frame after triggering the toolbar
			z-index: 2 !important;
		}
	}
}
</style>
