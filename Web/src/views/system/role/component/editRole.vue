<template>
	<div class="sys-role-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>{{ props.title }}</span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Character namecall" prop="name" :rules="[{ required: true, message: 'The role name cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.name" placeholder="Character namecall" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Character Encoding" prop="code" :rules="[{ required: true, message: 'Role code cannot be empty', trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.code" placeholder="Character Encoding" clearable :disabled="state.ruleForm.code == 'sys_admin' && state.ruleForm.id != undefined" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Sort">
							<el-input-number v-model="state.ruleForm.orderNo" placeholder="Sort" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="state">
							<el-radio-group v-model="state.ruleForm.status">
								<el-radio :value="1">enable</el-radio>
								<el-radio :value="2">Disable</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Remarks">
							<el-input v-model="state.ruleForm.remark" placeholder="Please enter the remark content" clearable type="textarea" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Menu permissions" v-loading="state.loading" class="tree-container">
							<el-tree
								ref="treeRef"
								:data="state.menuData"
								node-key="id"
								show-checkbox
								:props="{ children: 'children', label: 'title', class: treeNodeClass }"
								icon="ele-Menu"
								highlight-current
								default-expand-all
								style="height: 450px;overflow-y: auto;"
							/>
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

<script lang="ts" setup name="sysEditRole">
import { reactive, ref } from 'vue';
import type { ElTree } from 'element-plus';

import { getAPI } from '/@/utils/axios-utils';
import { SysMenuApi, SysRoleApi } from '/@/api-services/api';
import { SysMenu, UpdateRoleInput } from '/@/api-services/models';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const treeRef = ref<InstanceType<typeof ElTree>>();
const state = reactive({
	loading: false,
	isShowDialog: false,
	ruleForm: {} as UpdateRoleInput,
	menuData: [] as Array<SysMenu>, // Menu data
});

// Open pop-up window
const openDialog = async (row: any) => {
	state.menuData = await getAPI(SysMenuApi).apiSysMenuListGet(undefined, undefined, row?.tenantId).then((res) => res.data.result ?? []);
	ruleFormRef.value?.resetFields();
	treeRef.value?.setCheckedKeys([]); // Clear selected value
	state.ruleForm = JSON.parse(JSON.stringify(row));
	if (row.id != undefined) {
		var res = await getAPI(SysRoleApi).apiSysRoleOwnMenuListGet(row.id);
		setTimeout(() => {
			treeRef.value?.setCheckedKeys(res.data.result ?? []);
		}, 100);
	}
	state.isShowDialog = true;
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
		state.ruleForm.menuIdList = treeRef.value?.getCheckedKeys() as Array<number>; //.concat(treeRef.value?.getHalfCheckedKeys());
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysRoleApi).apiSysRoleUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysRoleApi).apiSysRoleAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// Leaf node peer display style
const treeNodeClass = (node: SysMenu) => {
	let addClass = true; // Add leaf node peer display style
	for (var key in node.children) {
		// If there are child nodes that are not leaf nodes, no style will be added.
		if (node.children[key].children?.length ?? 0 > 0) {
			addClass = false;
			break;
		}
	}
	return addClass ? 'penultimate-node' : '';
};

// Export object
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
.tree-container {
    :deep(.el-form-item__content) {
        border: 1px solid var(--el-border-color);
        border-radius: var(--el-input-border-radius, var(--el-border-radius-base));
        padding: 5px 0 5px 3px;
    }
    
}

.menu-data-tree {
	width: 100%;
	border: 1px solid var(--el-border-color);
	border-radius: var(--el-input-border-radius, var(--el-border-radius-base));
	padding: 5px;
}

:deep(.penultimate-node) {
	.el-tree-node__children {
		padding-left: 40px;
		white-space: pre-wrap;
		line-height: 100%;

		.el-tree-node {
			display: inline-block;
		}

		.el-tree-node__content {
			padding-left: 5px !important;
			padding-right: 5px;

			// .el-tree-node__expand-icon {
			// 	display: none;
			// }
		}
	}
}
</style>
