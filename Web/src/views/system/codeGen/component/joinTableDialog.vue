<template>
	<div class="sys-codeGenTree-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{state.dialogTitle}} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="library locator" prop="fkConfigId" :rules="[{ required: true, message: 'The library cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.fkConfigId" placeholder="Library name" filterable clearable @change="DbChanged()" class="w100">
								<el-option v-for="item in state.dbData" :key="item.configId" :label="item.dbNickName" :value="item.configId" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="database table" prop="fkTableName" :rules="[{ required: true, message: 'The data table cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.fkTableName" class="w100" filterable clearable @change="TableChanged()">
								<el-option v-for="item in state.tableData" :key="item.entityName" :label="item.tableName + ' [' + item.tableComment + ']'" :value="item.tableName" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="DisplayField" prop="fkDisplayColumnList" :rules="[{ required: true, message: 'Display field cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.fkDisplayColumnList" multiple filterable clearable class="w100">
								<el-option v-for="item in state.columnData" :key="item.propertyName" :label="item.propertyName + ' [' + item.columnComment + ']'" :value="item.propertyName" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="Value Field" prop="fkLinkColumnName" :rules="[{ required: true, message: 'The value field cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.fkLinkColumnName" filterable clearable class="w100">
								<el-option v-for="item in state.columnData" :key="item.propertyName" :label="item.propertyName + ' [' + item.columnComment + ']'" :value="item.propertyName" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" v-if="state.ruleForm.effectType == 'ApiTreeSelector'">
						<el-form-item label="Parent field" prop="pidColumn" :rules="[{ required: true, message: 'Parent field cannot be empty', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.pidColumn" filterable clearable class="w100">
								<el-option v-for="item in state.columnData" :key="item.propertyName" :label="item.propertyName + ' [' + item.columnComment + ']'" :value="item.propertyName" />
							</el-select>
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

<script lang="ts" setup name="sysCodeGenTree">
import { reactive, ref } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysCodeGenApi } from '/@/api-services/api';

const emits = defineEmits(['submitRefreshFk']);
const ruleFormRef = ref();
const state = reactive({
  dialogTitle: '' as string,
	isShowDialog: false,
	ruleForm: {} as any,
	dbData: [] as any,
	tableData: [] as any,
	columnData: [] as any,
});

const DbChanged = async () => {
	state.tableData = [];
	state.columnData = [];
	await getTableInfoList();
};

const TableChanged = async () => {
	state.columnData = [];
	await getColumnInfoList();
  state.ruleForm.pidColumn = undefined;
	state.ruleForm.fkDisplayColumnList = undefined;
  state.ruleForm.fkLinkColumnName = state.columnData.find(x => "True" === x.columnKey)?.columnName;
};

const getDbList = async () => {
  state.dbData = await getAPI(SysCodeGenApi).apiSysCodeGenDatabaseListGet().then(res => res.data.result ?? []);
};

const getTableInfoList = async () => {
	if (!state.ruleForm.fkConfigId) return;
  state.tableData = await getAPI(SysCodeGenApi)
      .apiSysCodeGenTableListConfigIdGet(state.ruleForm.fkConfigId)
      .then(res => res.data.result ?? []);
};

const getColumnInfoList = async () => {
	if (!state.ruleForm.fkConfigId || !state.ruleForm.fkTableName) return;
  state.columnData = await getAPI(SysCodeGenApi)
      .apiSysCodeGenColumnListByTableNameTableNameConfigIdGet(state.ruleForm.fkTableName, state.ruleForm.fkConfigId)
      .then(res => res.data.result ?? []);
};

// Open pop-up window
const openDialog = async (row: any, title: string) => {
  await getDbList();
  state.dialogTitle = title;
  state.isShowDialog = true;
  state.ruleForm = JSON.parse(JSON.stringify(row));
	if (row.fkConfigId) {
		await DbChanged();
    await TableChanged();
    state.ruleForm.pidColumn = row.pidColumn;
    state.ruleForm.fkLinkColumnName = row.fkLinkColumnName;
    state.ruleForm.fkDisplayColumnList = row.fkDisplayColumnList;
	}
};

// Close pop-up window
const closeDialog = () => {
  state.ruleForm.fkColumnNetType = state.columnData.find(x => x.columnName == state.ruleForm.fkLinkColumnName)?.netType;
  state.ruleForm.fkEntityName = state.tableData.find(x => x.tableName == state.ruleForm.fkTableName)?.entityName;
	if (state.ruleForm.effectType != 'ApiTreeSelector') state.ruleForm.pidColumn = null;
  emits('submitRefreshFk', state.ruleForm);
	cancel();
};

// Cancel
const cancel = () => {
	state.isShowDialog = false;
	state.dbData.value = [];
	state.tableData.value = [];
	state.columnData.value = [];
};

// submit
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		closeDialog();
	});
};

// Export object
defineExpose({ openDialog });
</script>
