<template>
	<div class="sys-jobTrigger-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Trigger number" prop="triggerId" :rules="[{ required: true, message: 'Trigger number cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.triggerId" placeholder="Trigger number" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Trigger type">
							<el-select v-model="state.ruleForm.triggerType" style="width: 100%">
								<el-option value="Furion.Schedule.PeriodTrigger" label="interval"></el-option>
								<el-option value="Furion.Schedule.CronTrigger" label="Cron expression"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" v-if="state.ruleForm.triggerType == 'Furion.Schedule.PeriodTrigger'">
						<el-form-item label="Interval time (ms)">
							<el-input-number v-model="periodValue" placeholder="interval" :min="1000" :step="1000" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" v-else>
						<el-form-item label="Cron expression">
							<el-input v-model="cronValue" placeholder="Cron expression">
								<template #append>
									<el-space :size="10" spacer="|">
										<el-dropdown style="color: inherit" trigger="click" @command="macroDropDownCommand">
											<el-button style="margin: 0px -10px 0px -20px; color: inherit"> Macro </el-button>
											<template #dropdown>
												<el-dropdown-menu>
													<el-dropdown-item v-for="(item, index) in macroData" :key="index" :command="item">
														<el-row style="width: 240px">
															<el-col :span="9">{{ item.key }}</el-col>
															<el-col :span="15">{{ item.description }}</el-col>
														</el-row>
													</el-dropdown-item>
												</el-dropdown-menu>
											</template>
										</el-dropdown>
										<el-button style="margin: 0px -20px 0px -10px; font-size: 14px" @click="state.showCronDialog = true">Cron expression</el-button>
									</el-space>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="start time">
							<el-date-picker v-model="state.ruleForm.startTime" type="datetime" placeholder="start time" style="width: 100%" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="end time">
							<el-date-picker v-model="state.ruleForm.endTime" type="datetime" placeholder="end time" style="width: 100%" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Maximum trigger count">
							<el-input-number v-model="state.ruleForm.maxNumberOfRuns" placeholder="Maximum trigger count" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Maximum number of errors">
							<el-input-number v-model="state.ruleForm.maxNumberOfErrors" placeholder="Maximum number of errors" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Number of retries">
							<el-input-number v-model="state.ruleForm.numRetries" placeholder="Number of retries" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Retry interval (ms)">
							<el-input-number v-model="state.ruleForm.retryTimeout" placeholder="Retry interval ms" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Start now">
							<el-radio-group v-model="state.ruleForm.startNow">
								<el-radio :value="true">Yes</el-radio>
								<el-radio :value="false">no</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Execute once at startup">
							<el-radio-group v-model="state.ruleForm.runOnStart">
								<el-radio :value="true">Yes</el-radio>
								<el-radio :value="false">no</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item>
							<template v-slot:label>
								<div>
									Reset trigger count
									<el-tooltip raw-content content="Whether to reset jobs with a maximum trigger count equal to one at startup<br/>Resolve the issue where jobs that have already triggered once due to persisted data do not execute again at startup" placement="top">
										<SvgIcon name="fa fa-question-circle-o" :size="15" style="vertical-align: middle" />
									</el-tooltip>
								</div>
							</template>
							<el-radio-group v-model="state.ruleForm.resetOnlyOnce">
								<el-radio :value="true">Yes</el-radio>
								<el-radio :value="false">no</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Description information" prop="description">
							<el-input v-model="state.ruleForm.description" placeholder="Description information" clearable type="textarea" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>

		<el-dialog v-model="state.showCronDialog" draggable :close-on-click-modal="false" class="scrollbar">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> Cron expression generator </span>
				</div>
			</template>
			<vcrontab id="vcrontab" @hide="state.showCronDialog = false" @fill="crontabFill" :expression="cronValue"></vcrontab>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditJobTrigger">
import { reactive, ref, computed } from 'vue';
import type { WritableComputedRef } from 'vue';
import { ElMessage } from 'element-plus';

import vcrontab from 'vcrontab-3';

import { getAPI } from '/@/utils/axios-utils';
import { SysJobApi } from '/@/api-services/api';
import { UpdateJobTriggerInput } from '/@/api-services/models';

// Macro identifier data structure
interface MacroData {
	key: string;
	description: string;
}

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdateJobTriggerInput,
	showCronDialog: false,
});

const macroData: MacroData[] = reactive([
	{ key: '@secondly', description: '0.0000000 per second' },
	{ key: '@minutely', description: '00 per minute' },
	{ key: '@hourly', description: 'Every hour 00:00' },
	{ key: '@daily', description: 'Every day 00:00:00' },
	{ key: '@monthly', description: '00:00:00 on the 1st of every month' },
	{ key: '@weekly', description: 'EverySunday 00:00:00' },
	{ key: '@yearly', description: 'Every January 1st 00:00:00' },
	{ key: '@workday', description: 'Every Monday to Friday 00:00:00' },
]);

// interval value
const periodValue: WritableComputedRef<number | undefined> = computed({
	get() {
		const defaultValue: number | undefined = undefined;
		// Trigger period is not a period, return default value
		if (state.ruleForm.triggerType != 'Furion.Schedule.PeriodTrigger') return defaultValue;
		if (!state.ruleForm.args) return defaultValue;

		const value: number | undefined = Number(state.ruleForm.args);
		if (Number.isNaN(value)) return defaultValue;

		return value;
	},
	set(value: number | undefined) {
		state.ruleForm.args = String(value);
	},
});

// cron expression value
const cronValue: WritableComputedRef<string> = computed({
	get() {
		const defaultValue = '';
		// Trigger period is not a period, return default value
		if (state.ruleForm.triggerType != 'Furion.Schedule.CronTrigger') return defaultValue;
		if (!state.ruleForm.args) return defaultValue;
		// Furion's cron expression has 2 input parameters
		const value = String(state.ruleForm.args);
		const parameters = value.split(',');
		if (parameters.length < 2) return defaultValue;
		else if (parameters.length == 2) {
			const cron = parameters[0].replace(new RegExp('"', 'gm'), '').trim();
			return cron;
		} else {
			const temp = value.substring(0, value.lastIndexOf(','));
			const cron = temp.replace(new RegExp('"', 'gm'), '').trim();
			return cron;
		}
	},
	set(value: string) {
		if (state.ruleForm.args == value) return;
		const newValue = value.trim();
		// For the second parameter value, please refer to https://furion.baiqian.ltd/docs/cron#2624-cronstringformat-%E6%A0%BC%E5%BC%8F%E5%8C%96
		let cronStringFormatValue = -1;
		// If it is a Macro identifier, use the default format
		if (newValue.startsWith('@'))
			cronStringFormatValue = 0; // Default format, writing order: minutes, hours, days, months, weeks
		else {
			if (newValue.split(' ').length == 6)
				cronStringFormatValue = 2; // With seconds format, writing order: seconds, minutes, hours, days, months, weeks
			else cronStringFormatValue = 3; // With seconds and year format, writing order: seconds, minutes, hours, days, months, anniversary
		}
		state.ruleForm.args = `"${newValue}",${cronStringFormatValue}`;
	},
});

// Open pop-up window
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
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
		if (state.ruleForm.triggerType == 'Furion.Schedule.PeriodTrigger' && !periodValue.value) {
			ElMessage.warning('intervaltimecannot benull');
			return;
		} else if (state.ruleForm.triggerType == 'Furion.Schedule.CronTrigger' && !cronValue.value) {
			ElMessage.warning('Cron expressioncannot benull');
			return;
		}

		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysJobApi).apiSysJobUpdateJobTriggerPost(state.ruleForm);
		} else {
			await getAPI(SysJobApi).apiSysJobAddJobTriggerPost(state.ruleForm);
		}
		closeDialog();
	});
};

// cron form determines the value after
const crontabFill = (value: string | null | undefined) => {
	cronValue.value = value == null || value == undefined ? '' : value;
};

// macro drop-down selection callback
const macroDropDownCommand = (item: MacroData) => {
	cronValue.value = item.key;
};

// Export object
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
#vcrontab {
	:deep(.el-select) {
		min-width: 300px;
	}
}
</style>
