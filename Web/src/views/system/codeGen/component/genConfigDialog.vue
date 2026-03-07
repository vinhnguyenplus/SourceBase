<template>
	<div class="sys-codeGenConfig-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="1500px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> Generate Configuration </span>
				</div>
			</template>
			<el-table :data="state.tableData" style="width: 100%; height: 100%;" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="propertyName" label="Entity attribute" show-overflow-tooltip />
				<el-table-column prop="columnComment" label="Description" show-overflow-tooltip>
					<template #default="scope">
						<el-input v-model="scope.row.columnComment" autocomplete="off" />
					</template>
				</el-table-column>
				<el-table-column prop="netType" label="data type" width="130" show-overflow-tooltip />
				<el-table-column prop="effectType" label="Type of effect" width="150" show-overflow-tooltip>
					<template #default="scope">
						<div class="effect-type-container">
              <g-sys-dict v-model="scope.row.effectType" code="code_gen_effect_type" render-as="select" @change="effectTypeChange(scope.row, scope.$index)" :disabled="judgeColumns(scope.row)" class="m-2" />
              <el-button
                  v-if="['ApiTreeSelector','ForeignKey'].some(x => scope.row.effectType == x)"
                  @click="effectTypeChange(scope.row, scope.$index)"
                  type="warning"
                  :icon="Edit"
                  link />
						</div>
					</template>
				</el-table-column>
				<el-table-column prop="dictTypeCode" label="dictionary" width="150" show-overflow-tooltip>
					<template #default="scope">
						<el-select v-model="scope.row.dictTypeCode" :disabled="effectTypeEnable(scope.row)" class="m-2">
							<el-option
							v-for="item in state.selectDataMap[scope.row.effectType] ?? []"
							:key="item.code"
							:label="item.name"
							:value="item.code" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column prop="whetherTable" label="Display" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.whetherTable" true-value="Y" false-value="N" />
					</template>
				</el-table-column>
				<el-table-column prop="whetherAddUpdate" label="Additions and changes" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.whetherAddUpdate" true-value="Y" false-value="N" :disabled="judgeColumns(scope.row)" />
					</template>
				</el-table-column>
				<el-table-column prop="whetherImport" label="import" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.whetherImport" true-value="Y" false-value="N" :disabled="judgeColumns(scope.row)" />
					</template>
				</el-table-column>
				<el-table-column prop="whetherRequired" label="Required" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.whetherRequired" true-value="Y" false-value="N" :disabled="judgeColumns(scope.row)" />
					</template>
				</el-table-column>
				<el-table-column prop="whetherSortable" label="Sortable" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-checkbox v-model="scope.row.whetherSortable" true-value="Y" false-value="N" />
					</template>
				</el-table-column>
				<el-table-column prop="whetherQuery" label="Query" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-switch v-model="scope.row.whetherQuery" active-value="Y" inactive-value="N" />
					</template>
				</el-table-column>
				<el-table-column prop="queryType" label="Query method" width="110" align="center" show-overflow-tooltip>
					<template #default="scope">
            <g-sys-dict v-model="scope.row.queryType" code="code_gen_query_type" render-as="select" class="m-2" placeholder="Select" :disabled="!scope.row.whetherQuery" />
					</template>
				</el-table-column>
				<el-table-column prop="orderNo" label="Sort" width="80" show-overflow-tooltip>
					<template #default="scope">
						<el-input v-model="scope.row.orderNo" autocomplete="off" type="number" />
					</template>
				</el-table-column>
			</el-table>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>

		<JoinTableDialog ref="joinTableDialogRef" @submitRefreshFk="submitRefreshFk" />
	</div>
</template>

<script lang="ts" setup name="sysCodeGenConfig">
import { onMounted, reactive, ref } from 'vue';
import { Edit } from '@element-plus/icons-vue';
import { useUserInfo } from "/@/stores/userInfo";
import { getAPI } from '/@/utils/axios-utils';
import { SysCodeGenConfigApi, SysDictTypeApi } from '/@/api-services/api';
import JoinTableDialog from '/src/views/system/codeGen/component/joinTableDialog.vue';

const emits = defineEmits(['handleQuery']);
const joinTableDialogRef = ref();
const state = reactive({
	isShowDialog: false,
	loading: false,
  selectDataMap: {} as any,
  tableData: [] as any[]
});

onMounted(async () => {
  state.selectDataMap.DictSelector = [];
  state.selectDataMap.EnumSelector = [];
  state.selectDataMap.ConstSelector = useUserInfo().constList;
  const dictList = await getAPI(SysDictTypeApi).apiSysDictTypeListGet().then(res => res.data.result ?? []);
  for (const item of dictList) state.selectDataMap[item.code?.endsWith('Enum') ? 'EnumSelector' : 'DictSelector'].push(item);
});

// Update primary key
const submitRefreshFk = (data: any) => {
	state.tableData[data.index] = data;
};

// Control type changes
const effectTypeChange = (data: any, index: number) => {
	if (['ForeignKey', 'ApiTreeSelector'].some(type => data.effectType == type)) {
    openJoinTableDialog(data, 'ForeignKey' === data.effectType ? 'foreign keyConfiguration' : 'Tree Selector Configuration', index);
	} else if (['DictSelector', 'ConstSelector', 'EnumSelector'].some(type => data.effectType === type)) {
		data.dictTypeCode = '';
	}
};

// Query operation
const handleQuery = async (row: any) => {
	state.loading = true;
	state.tableData = await getAPI(SysCodeGenConfigApi).apiSysCodeGenConfigListGet(undefined, row.id).then(res => res.data.result ?? []);
	state.loading = false;
};

// Determine whether (used for whether selection or input can be selected, etc.)
function judgeColumns(data: any) {
	return data.whetherCommon == "Y" || data.columnKey === 'True';
}

function effectTypeEnable(data: any) {
  return !['Radio', 'Checkbox', 'DictSelector', 'ConstSelector', 'EnumSelector'].some((e: any) => e === data.effectType)
}

// Open pop-up window
const openDialog = (row: any) => {
	handleQuery(row);
	state.isShowDialog = true;
};

// Open pop-up window
const openJoinTableDialog = (row: any, title: string, index: number) => {
  row.index = index;
  joinTableDialogRef.value.openDialog(row, title);
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
const submit = async () => {
	state.loading = true;
	await getAPI(SysCodeGenConfigApi).apiSysCodeGenConfigUpdatePost(state.tableData);
	state.loading = false;
	closeDialog();
};

// Export object
defineExpose({ openDialog });
</script>
<style lang="scss" scoped>
.effect-type-container {
	display: flex;
	align-items: center;
}

:deep(.el-dialog__body) {
	height: calc(100vh - 160px) !important;
}
</style>
