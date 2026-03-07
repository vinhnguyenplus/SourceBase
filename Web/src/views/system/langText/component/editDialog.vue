<script lang="ts" name="sysLangText" setup>
import { ref, reactive, onMounted } from "vue";
import { ElMessage } from "element-plus";
import type { FormRules } from "element-plus";
import { formatDate } from '/@/utils/formatTime';
import { useSysLangTextApi } from '/@/api/system/sysLangText';

//Function passed from parent for callback
const emit = defineEmits(["reloadTable"]);
const sysLangTextApi = useSysLangTextApi();
const ruleFormRef = ref();

const state = reactive({
	title: '',
	loading: false,
	showDialog: false,
	ruleForm: {} as any,
	stores: {},
	dropdownData: {} as any,
});

// Add other rules yourself
const rules = ref<FormRules>({
  entityName: [{required: true, message: 'Please select the name of the affiliated entity!', trigger: 'blur',},],
  entityId: [{required: true, message: 'Please select the entity ID to which you belong!', trigger: 'blur',},],
  fieldName: [{required: true, message: 'Please select a field name!', trigger: 'blur',},],
  langCode: [{required: true, message: 'Please select the language code!', trigger: 'blur',},],
  content: [{required: true, message: 'Please select translation content!', trigger: 'blur',},],
});

// When the page loads
onMounted(async () => {
});

// Open pop-up window
const openDialog = async (row: any, title: string) => {
	state.title = title;
	row = row ?? {  };
	state.ruleForm = row.id ? await sysLangTextApi.detail(row.id).then(res => res.data.result) : JSON.parse(JSON.stringify(row));
	state.showDialog = true;
};

// Close pop-up window
const closeDialog = () => {
	emit("reloadTable");
	state.showDialog = false;
};

// submit
const submit = async () => {
	ruleFormRef.value.validate(async (isValid: boolean, fields?: any) => {
		if (isValid) {
			let values = state.ruleForm;
			await sysLangTextApi[state.ruleForm.id ? 'update' : 'add'](values);
			closeDialog();
		} else {
			ElMessage({
				message: `The form failed to verify at ${Object.keys(fields).length}, please modify it before submitting.`,
				type: "error",
			});
		}
	});
};

//Expose properties or functions to parent components
defineExpose({ openDialog });
</script>
<template>
	<div class="sysLangText-container">
		<el-dialog v-model="state.showDialog" :width="800" draggable :close-on-click-modal="false">
			<template #header>
				<div style="color: #fff">
					<span>{{ state.title }}</span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" :rules="rules">
				<el-row :gutter="35">
					<el-form-item v-show="false">
						<el-input v-model="state.ruleForm.id" />
					</el-form-item>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" >
						<el-form-item label="Name of the affiliated entity" prop="entityName">
							<el-input v-model="state.ruleForm.entityName" placeholder="Please enter the name of the entity you belong to" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" >
						<el-form-item label="Associated Entity ID" prop="entityId">
							<el-input v-model="state.ruleForm.entityId" placeholder="Please enter the affiliated entity ID" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" >
						<el-form-item label="Field Name" prop="fieldName">
							<el-input v-model="state.ruleForm.fieldName" placeholder="Please enter a field name" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" >
						<el-form-item label="Language code" prop="langCode">
							<el-input v-model="state.ruleForm.langCode" placeholder="Please enter language code" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" >
						<el-form-item label="Translate content" prop="content">
							<el-input v-model="state.ruleForm.content" placeholder="Please enter the content to be translated" maxlength="255" show-word-limit clearable />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="() => state.showDialog = false">Cancel</el-button>
					<el-button @click="submit" type="primary" v-reclick="1000">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<style lang="scss" scoped>
:deep(.el-select), :deep(.el-input-number) {
  width: 100%;
}
</style>