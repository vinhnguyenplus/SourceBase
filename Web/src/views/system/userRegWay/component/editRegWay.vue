<template>
	<div class="sys-tenant-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Plan Name" prop="name" :rules="[{ required: true, message: 'Scheme name cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.name" placeholder="Plan Name" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Account Type" prop="posId" :rules="[{ required: true, message: 'Account type cannot be empty', trigger: 'blur' }]">
							<g-sys-dict
									v-model="state.ruleForm.accountType"
									:on-item-filter="(data: any) => !['SuperAdmin','SysAdmin'].includes(data.name)"
									code="AccountTypeEnum"
									render-as="select"
							/>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Bind role" prop="roleId" :rules="[{ required: true, message: 'Role cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.roleId" placeholder="Bind role" clearable class="w100">
								<el-option :label="item.name" :value="item.id" v-for="(item, index) in state.roleData" :key="index" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Bind Organization" prop="orgId" :rules="[{ required: true, message: 'Organization cannot be empty', trigger: 'blur' }]">
							<el-cascader :options="state.orgData" :props="cascaderConfig" v-model="state.ruleForm.orgId" placeholder="Bind Organization" clearable filterable class="w100" >
								<template #default="{ node, data }">
									<span>{{ data.name }}</span>
									<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
								</template>
							</el-cascader>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Bind position" prop="posId" :rules="[{ required: true, message: 'Position cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.posId" placeholder="Bind position" clearable class="w100">
								<el-option :label="item.name" :value="item.id" v-for="(item, index) in state.posData" :key="index" />
							</el-select>
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
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="() => state.isShowDialog = false">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysEditTenant">
import { onMounted, reactive, ref } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysOrgApi, SysPosApi, SysRoleApi, SysUserRegWayApi } from '/@/api-services/api';
import { OrgTreeOutput, RoleOutput, SysPos, UpdateUserRegWayInput } from '/@/api-services/models';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	loading: false,
	selectedTabName: '0',
	isShowDialog: false,
	file: undefined as any,
	ruleForm: {} as UpdateUserRegWayInput,
	orgData: [] as Array<OrgTreeOutput>,
	posData: [] as Array<SysPos>, // Job data
	roleData: [] as Array<RoleOutput>, // character data
});

onMounted(async () => {
	state.loading = true;
	state.posData = await getAPI(SysPosApi).apiSysPosListGet().then(res => res.data.result ?? []);
	state.roleData = await getAPI(SysRoleApi).apiSysRoleListGet().then(res => res.data.result ?? []);
	state.orgData = await getAPI(SysOrgApi).apiSysOrgTreeGet(0).then(res => res.data.result ?? []);
	state.loading = false;
});

// Cascading selector configuration options
const cascaderConfig = {
	checkStrictly: true,
	emitPath: false,
	value: 'id',
	label: 'name',
	expandTrigger: 'hover'
};

// Open pop-up window
const openDialog = async (row: any) => {
	state.roleData = (row?.tenantId ? state.roleData?.filter((e) => e.tenantId === row.tenantId) : state.roleData) ?? [];
	state.posData = (row?.tenantId ? state.posData?.filter((e) => e.tenantId === row.tenantId) : state.posData) ?? [];
	state.orgData = (row?.tenantId ? state.orgData?.filter((e) => e.tenantId === row.tenantId) : state.orgData) ?? [];
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// Close pop-up window
const closeDialog = () => {
	emits('handleQuery');
	state.isShowDialog = false;
};

// submit
const submit = async () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.ruleForm.id) {
			await getAPI(SysUserRegWayApi).apiSysUserRegWayUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysUserRegWayApi).apiSysUserRegWayAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
