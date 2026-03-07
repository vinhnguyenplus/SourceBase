<template>
	<div class="sys-region-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<!-- <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Superior Name">
							<el-cascader
								:options="regionData"
								:props="cascaderProps"
								placeholder="Please select the name of the supervisor"
								clearable
								class="w100"
								v-model="ruleForm.pid"
							>
								<template #default="{ node, data }">
									<span>{{ data.name }}</span>
									<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
								</template>
							</el-cascader>
						</el-form-item>
					</el-col> -->
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Superior administrative ID" prop="pid" v-show="false" :rules="[{ required: true, message: 'The superior administrative Id cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.pid" placeholder="Superior administrative ID or superior administrative code or default 0" clearable />
						</el-form-item>
                        <el-form-item label="Higher administration" v-if="!state.ruleForm.id">
                            <!-- <span>{{state.parentNamePath}}</span> -->
                            <el-input v-model="state.parentNamePath" disabled></el-input>
                        </el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Administrative name" prop="name" :rules="[{ required: true, message: 'Administrative name cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.name" placeholder="Administrative name" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Administrative code" prop="code" :rules="[{ required: true, message: 'Administrative code cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.code" placeholder="Administrative code" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Area code" prop="cityCode">
							<el-input v-model="state.ruleForm.cityCode" placeholder="Area code" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Sort">
							<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Remarks">
							<el-input v-model="state.ruleForm.remark" placeholder="Please enter the remark content" clearable type="textarea" />
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

<script lang="ts" setup name="sysEditRegion">
import { reactive, ref } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysRegionApi } from '/@/api-services/api';
import { UpdateRegionInput } from '/@/api-services/models';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdateRegionInput,
    parentNamePath: 'Top'
});

// Open pop-up window
const openDialog = (row: any, parentNamePath?: string) => {
    if(parentNamePath) state.parentNamePath = parentNamePath;

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
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysRegionApi).apiSysRegionUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysRegionApi).apiSysRegionAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
