<template>
	<div class="editSchedule-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="8" :sm="8" :md="8" :lg="8" :xl="8" class="mb20 time-padding-right">
						<el-form-item label="schedule time" prop="scheduleTime" :rules="[{ required: true, message: 'Schedule time cannot be empty', trigger: 'blur' }]">
							<el-date-picker v-model="state.ruleForm.scheduleTime" type="datetime" placeholder="Please select a schedule date" format="YYYY-MM-DD" value-format="YYYY-MM-DD HH:mm:ss" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="5" :sm="5" :md="5" :lg="5" :xl="5" class="mb20 time-padding">
						<el-form-item prop="startTime" :rules="[{ required: true, message: 'Start time cannot be empty', trigger: 'blur' }]">
							<el-time-select v-model="state.ruleForm.startTime" format="HH:mm" start="00:00" end="23:45" step="00:15" class="w100" clearable @change="ChangeEndTime()" />
						</el-form-item>
					</el-col>
					<span>to</span>
					<el-col :xs="5" :sm="5" :md="5" :lg="5" :xl="5" class="mb20 time-padding">
						<el-form-item prop="endTime" :rules="[{ required: true, message: 'End time cannot be empty', trigger: 'blur' }]">
							<el-time-select v-model="state.ruleForm.endTime" :min-time="state.ruleForm.startTime" format="HH:mm" start="00:00" end="23:45" step="00:15" class="w100" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Schedule content" prop="content" :rules="[{ required: true, message: 'Content cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.content" placeholder="content content" clearable type="textarea" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button v-if="state.showRemove" @click="remove">Delete</el-button>
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="editSchedule">
import { onMounted, reactive, ref } from 'vue';
import { dayjs, ElMessageBox, ElMessage } from 'element-plus';

import { getAPI } from '/@/utils/axios-utils';
import { SysScheduleApi } from '/@/api-services/api';
import { SysSchedule, UpdateScheduleInput } from '/@/api-services/models';

const props = defineProps({
	title: String,
	userScheduleData: Array<SysSchedule>,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	showRemove: false,
	ruleForm: {} as any,
});

// Page initialization
onMounted(() => {});

// Open pop-up window
const openDialog = (row: any, showRemove: boolean = false) => {
	ruleFormRef.value?.resetFields();
	state.showRemove = showRemove;

	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.ruleForm.scheduleTime = dayjs(state.ruleForm.scheduleTime ?? new Date()).format('YYYY-MM-DD HH:mm:ss');
	state.isShowDialog = true;
};

// Close pop-up window
const closeDialog = () => {
	emits('handleQuery', true);
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
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysScheduleApi).apiSysScheduleUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysScheduleApi).apiSysScheduleAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// delete
const remove = () => {
	ElMessageBox.confirm(`Are you sure to delete?`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysScheduleApi).apiSysScheduleDeletePost(state.ruleForm);
			closeDialog();
			ElMessage.success('Operation successful');
		})
		.catch(() => {});
};

// start time change
const ChangeEndTime = () => {
	// Conversion date
	var timeStr = state.ruleForm.startTime;
	var parts = timeStr.split(':');
	var hours = parseInt(parts[0], 10);
	var minutes = parseInt(parts[1], 10);

	var startTime = new Date();
	startTime.setHours(hours);
	startTime.setMinutes(minutes);

	if (startTime.getHours() < 23) {
		startTime.setHours(startTime.getHours() + 1);
		state.ruleForm.endTime = dayjs(startTime).format('HH:mm');
	} else {
		state.ruleForm.endTime = '23:45';
	}
};

// Export object
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
.editSchedule-container {
	.no-pre-icon {
		color: red;
	}
}
:v-deep(.el-select__prefix) {
	display: none !important;
}
.time-padding-right {
	padding-right: 1px !important;
}
.time-padding {
	padding-left: 10px !important;
	padding-right: 10px !important;
}
</style>
