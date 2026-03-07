<!--
// JsonEditor component, used to edit json-like format data to prevent incorrect format input of configuration data
// Usage example:
<template>
	<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
		<el-form-item label="content" prop="content">
			<JsonEditor ref="jsonEditorRef" v-model:jsonObj="ruleForm.content"></JsonEditor>
		</el-form-item>
	</el-col>
</template>
<script lang="ts" setup>
import JsonEditor from '/@/components/jsonEditor/index.vue';
</script>
-->

<template>
	<!-- Control the display of JsonEditorVue based on isJsonValid -->
	<JsonEditorVue ref="jsonEditorVueRef" v-show="isJsonValid" v-model="internalJsonObj" mode="text" style="width: 100%" />
</template>

<script setup lang="ts" name="jsonEditor">
import { ref, useTemplateRef, watch, onMounted, computed } from 'vue';
import JsonEditorVue from 'json-editor-vue';

const props = defineProps({
	jsonObj: {
		type: null,
		default: null, // allowed to be null
		required: false, // Not required
	},
});
const jsonEditorVueRef = useTemplateRef('jsonEditorVueRef');
const internalJsonObj = ref(props.jsonObj);
const emit = defineEmits(['update:jsonObj']);

// Calculate attributes to determine whether jsonObj is a valid JSON string. Here is a simple judgment to prevent components from being frequently hidden and displayed when the input box loses focus.
const isJsonValid = computed(() => {
	try {
		if (internalJsonObj.value && (internalJsonObj.value.startsWith('{') || internalJsonObj.value.startsWith('[')) && (internalJsonObj.value.endsWith('}') || internalJsonObj.value.endsWith(']'))) {
			return true;
		}
	} catch {
		return false;
	}
	return false;
});

// Monitor internal JSON object changes and trigger external updates
watch(internalJsonObj, () => {
	emit('update:jsonObj', internalJsonObj.value);
});

// Listen to external string changes and update the internal JSON object
watch(
	() => props.jsonObj,
	(newVal) => {
		internalJsonObj.value = newVal;
	}
);

onMounted(() => {
	jsonEditorVueRef.value.jsonEditor.focus();
});
</script>
