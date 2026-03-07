<script lang="ts" name="sysLang" setup>
import { ref, reactive, onMounted } from "vue";
import { ElMessage } from "element-plus";
import type { FormRules } from "element-plus";
import { formatDate } from '/@/utils/formatTime';
import { getAPI } from '/@/utils/axios-utils';
import { SysLangApi } from '/@/api-services/api';

//Function passed from parent for callback
const emit = defineEmits(["reloadTable"]);
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
	name: [{ required: true, message: 'Please select a language name!', trigger: 'blur', },],
	code: [{ required: true, message: 'Please select the language code!', trigger: 'blur', },],
	isoCode: [{ required: true, message: 'Please select an ISO language code!', trigger: 'blur', },],
	urlCode: [{ required: true, message: 'Please select a URL language code!', trigger: 'blur', },],
	direction: [{ required: true, message: 'Please select the writing direction!', trigger: 'blur', },],
	dateFormat: [{ required: true, message: 'Please select a date format!', trigger: 'blur', },],
	timeFormat: [{ required: true, message: 'Please select a time format!', trigger: 'blur', },],
	weekStart: [{ required: true, message: 'Please select the starting day of the week!', trigger: 'blur', },],
	grouping: [{ required: true, message: 'Please select a grouping symbol!', trigger: 'blur', },],
	decimalPoint: [{ required: true, message: 'Please select a decimal point symbol!', trigger: 'blur', },],
	active: [{ required: true, message: 'Please choose whether to enable it!', trigger: 'blur', },],
});

// When the page loads
onMounted(async () => {
});

// Open pop-up window
const openDialog = async (row: any, title: string) => {
	state.title = title;
	row = row ?? { direction: 1, weekStart: 7, active: false };
	state.ruleForm = row.id ? await getAPI(SysLangApi).apiSysLangDetailGet(row.id).then(res => res.data.result) : JSON.parse(JSON.stringify(row));
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
			if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
				await getAPI(SysLangApi).apiSysLangUpdatePost(state.ruleForm);
			} else {
				await getAPI(SysLangApi).apiSysLangAddPost(state.ruleForm);
			}
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
	<div class="sysLang-container">
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
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Language name" prop="name">
							<el-input v-model="state.ruleForm.name" placeholder="Please enter language name" maxlength="255"
								show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Language code" prop="code">
							<el-input v-model="state.ruleForm.code" placeholder="Please enter language code" maxlength="255"
								show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="ISO language code" prop="isoCode">
							<el-input v-model="state.ruleForm.isoCode" placeholder="Please enter the ISO language code" maxlength="255"
								show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="URL language code" prop="urlCode">
							<el-input v-model="state.ruleForm.urlCode" placeholder="Please enter the URL language code" maxlength="255"
								show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Writing direction" prop="direction">
							<g-sys-dict v-model="state.ruleForm.direction" code="DirectionEnum" render-as="select"
								placeholder="Please select writing direction" clearable filterable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="date format" prop="dateFormat">
							<el-input v-model="state.ruleForm.dateFormat" placeholder="Please enter the date format" maxlength="255"
								show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="time format" prop="timeFormat">
							<el-input v-model="state.ruleForm.timeFormat" placeholder="Please enter the time format" maxlength="255"
								show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Start day of the week" prop="weekStart">
							<g-sys-dict v-model="state.ruleForm.weekStart" code="WeekEnum" render-as="select"
								placeholder="Please chooseStart day of the week" clearable filterable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Grouping symbols" prop="grouping">
							<el-input v-model="state.ruleForm.grouping" placeholder="Please enter grouping symbol" maxlength="255"
								show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Decimal point symbol" prop="decimalPoint">
							<el-input v-model="state.ruleForm.decimalPoint" placeholder="Please enter the decimal point symbol" maxlength="255"
								show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="thousands separator" prop="thousandsSep">
							<el-input v-model="state.ruleForm.thousandsSep" placeholder="Please enter thousandth separator" maxlength="255"
								show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Enable or not" prop="active">
							<el-switch v-model="state.ruleForm.active" active-text="Yes" inactive-text="no" />
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
:deep(.el-select),
:deep(.el-input-number) {
	width: 100%;
}
</style>