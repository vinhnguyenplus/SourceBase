<template>
	<div class="sys-grantMenu-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="769px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> Authorized Tenant Menu </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" v-loading="state.loading">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl1="24">
						<el-form-item prop="orgIdList">
							<el-tree
								ref="treeRef"
								:data="state.menuData"
								node-key="id"
								show-checkbox
								:props="{ children: 'children', label: 'title', class: treeNodeClass }"
								icon="ele-Menu"
								highlight-current
								default-expand-all>
								<template #default="{ node, data }">
									<span :title="node.label+data.id">{{ node.label }}</span>
									<span v-if="data.path" style="margin-left: 5px!important;">
                    <el-tag effect="plain" type="warning" :title="data.component">{{data.path}}</el-tag>
                  </span>
								</template>
							</el-tree>
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

<script lang="ts" setup name="sysGrantMenu">
import { reactive, ref } from 'vue';
import type { ElTree } from 'element-plus';

import { getAPI } from '/@/utils/axios-utils';
import { SysMenuApi, SysTenantApi } from '/@/api-services/api';
import { SysMenu } from '/@/api-services/models';

const treeRef = ref<InstanceType<typeof ElTree>>();
const state = reactive({
	loading: false,
	isShowDialog: false,
	ruleForm: {
		id: 0,
    appId: 0,
		menuIdList: [] as any, // menu collection
	},
	menuData: [] as any, // Menu data
});

// Open pop-up window
const openDialog = async (row: any) => {
	treeRef.value?.setCheckedKeys([]); // Clear the selected nodes first
	state.ruleForm = row;
  state.menuData = await getAPI(SysMenuApi).apiSysMenuListGet().then(res => res.data.result);
	const menuIds = await getAPI(SysTenantApi).apiSysTenantTenantMenuListGet(row.id).then(res => res.data.result);
	setTimeout(() => {
		// Delayed delivery of data
		treeRef.value?.setCheckedKeys(menuIds ?? []);
	}, 100);
	state.isShowDialog = true;
};

// Cancel
const cancel = () => {
	state.isShowDialog = false;
};

// submit
const submit = async () => {
	state.ruleForm.menuIdList = treeRef.value?.getCheckedKeys() as Array<number>;
	await getAPI(SysTenantApi).apiSysTenantGrantMenuPost(state.ruleForm);
	state.isShowDialog = false;
};

// Leaf node peer display style
const treeNodeClass = (node: SysMenu) => {
	let addClass = true; // Add leaf node peer display style
	for (const key in node.children) {
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
