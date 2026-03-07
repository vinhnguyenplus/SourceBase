<template>
	<div class="sys-dictData-container">
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
						<el-form-item label="Display Text" prop="label" :rules="[{ required: true, message: 'Display text cannot be empty', trigger: 'blur' }]">
							<g-multi-lang-Input entityName="SysDictData" fieldName="Label" :entityId="state.ruleForm.id" v-model="state.ruleForm.label" placeholder="Display Text" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Dictionary value" prop="value" :rules="[{ required: true, message: 'Dictionary value cannot be empty', trigger: 'blur' } ]">
							<el-input v-model="state.ruleForm.value" placeholder="Dictionary value" :disabled="state.isSysFlag" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Encoding" prop="code">
							<el-input v-model="state.ruleForm.code" placeholder="Encoding" :disabled="state.isSysFlag" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Tag type">
							<el-radio-group v-model="state.ruleForm.tagType">
								<el-radio value="primary"><el-tag type="primary">theme color</el-tag></el-radio>
								<el-radio value="success"><el-tag type="success">success</el-tag></el-radio>
								<el-radio value="info"><el-tag type="info">info</el-tag></el-radio>
								<el-radio value="warning"><el-tag type="warning">warning</el-tag></el-radio>
								<el-radio value="danger"><el-tag type="danger">danger</el-tag></el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Style" prop="styleSetting">
							<el-input v-model="state.ruleForm.styleSetting" placeholder="Style" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Style (Class)" prop="classSetting">
							<el-input v-model="state.ruleForm.classSetting" placeholder="Style (Class)" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="state">
							<el-radio-group v-model="state.ruleForm.status" :disabled="state.isSysFlag">
								<el-radio :value="1">enable</el-radio>
								<el-radio :value="2">Disable</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Sort">
							<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Remarks">
							<el-input v-model="state.ruleForm.remark" placeholder="Please enter the remark content" clearable type="textarea" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Expand data">
							<el-input v-model="state.ruleForm.extData" placeholder="Please enter extended data" clearable type="textarea" :rows="6" />
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
	</div>
</template>

<script lang="ts" setup name="sysEditDictData">
import { reactive, ref } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysDictDataApi } from '/@/api-services/api';
import { UpdateDictDataInput } from '/@/api-services/models';

const props = defineProps({
	title: String,
	dictTypeId: Number,
});
const emits = defineEmits(['handleQuery', 'handleUpdate']);
const ruleFormRef = ref();
const state = reactive({
	isSysFlag: false,
	isShowDialog: false,
	ruleForm: {} as UpdateDictDataInput,
});

// Open pop-up window
const openDialog = (row: any) => {
	if (row.dictType?.sysFlag) 
		state.isSysFlag = row.dictType.sysFlag !== 2;
	else
		state.isSysFlag = false;
	state.ruleForm = JSON.parse(JSON.stringify(row));
	if (JSON.stringify(row) == '{}') {
		state.ruleForm.dictTypeId = props.dictTypeId;
	}
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
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysDictDataApi).apiSysDictDataUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysDictDataApi).apiSysDictDataAddPost(state.ruleForm);
		}
		emits('handleUpdate');
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
