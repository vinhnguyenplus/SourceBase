<script lang="ts" setup name="sysLangText">
import { ref, reactive, onMounted } from "vue";
import { auth } from '/@/utils/authFunction';
import { ElMessageBox, ElMessage } from "element-plus";
import { downloadStreamFile } from "/@/utils/download";
import { useSysLangTextApi } from '/@/api/system/sysLangText';
import editDialog from '/@/views/system/langText/component/editDialog.vue'
import printDialog from '/@/views/system/print/component/hiprint/preview.vue'
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import ImportData from "/@/components/table/importData.vue";

const sysLangTextApi = useSysLangTextApi();
const printDialogRef = ref();
const editDialogRef = ref();
const importDataRef = ref();
const state = reactive({
  exportLoading: false,
  tableLoading: false,
  stores: {},
  showAdvanceQueryUI: false,
  dropdownData: {} as any,
  selectData: [] as any[],
  tableQueryParams: {} as any,
  tableParams: {
    page: 1,
    pageSize: 20,
    total: 0,
    field: 'createTime', // Default sort field
    order: 'descending', // Sorting direction
    descStr: 'descending', // Key characters for sorting in descending order
  },
  tableData: [],
});

// When the page loads
onMounted(async () => {
});

// Query operation
const handleQuery = async (params: any = {}) => {
  state.tableLoading = true;
  state.tableParams = Object.assign(state.tableParams, params);
  const result = await sysLangTextApi.page(Object.assign(state.tableQueryParams, state.tableParams)).then(res => res.data.result);
  state.tableParams.total = result?.total;
  state.tableData = result?.items ?? [];
  state.tableLoading = false;
};

// Column sort
const sortChange = async (column: any) => {
  state.tableParams.field = column.prop;
  state.tableParams.order = column.order;
  await handleQuery();
};

// delete
const delSysLangText = (row: any) => {
  ElMessageBox.confirm(`ConfirmwantDelete??`, "Prompt", {
    confirmButtonText: "Confirm",
    cancelButtonText: "Cancel",
    type: "warning",
  }).then(async () => {
    await sysLangTextApi.delete({ id: row.id });
    handleQuery();
    ElMessage.success("Deleted successfully");
  }).catch(() => {});
};

// Batch delete
const batchDelSysLangText = () => {
  ElMessageBox.confirm(`Are you sure you want to delete ${state.selectData.length} records?`, "Prompt", {
    confirmButtonText: "Confirm",
    cancelButtonText: "Cancel",
    type: "warning",
  }).then(async () => {
    await sysLangTextApi.batchDelete(state.selectData.map(u => ({ id: u.id }) )).then(res => {
      ElMessage.success(`Successfully deleted ${res.data.result} records in bulk`);
      handleQuery();
    });
  }).catch(() => {});
};

// Export data
const exportSysLangTextCommand = async (command: string) => {
  try {
    state.exportLoading = true;
    if (command === 'select') {
      const params = Object.assign({}, state.tableQueryParams, state.tableParams, { selectKeyList: state.selectData.map(u => u.id) });
      await sysLangTextApi.exportData(params).then(res => downloadStreamFile(res));
    } else if (command === 'current') {
      const params = Object.assign({}, state.tableQueryParams, state.tableParams);
      await sysLangTextApi.exportData(params).then(res => downloadStreamFile(res));
    } else if (command === 'all') {
      const params = Object.assign({}, state.tableQueryParams, state.tableParams, { page: 1, pageSize: 99999999 });
      await sysLangTextApi.exportData(params).then(res => downloadStreamFile(res));
    }
  } finally {
    state.exportLoading = false;
  }
}

handleQuery();
</script>
<template>
  <div class="sysLangText-container" v-loading="state.exportLoading">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }"> 
      <el-form :model="state.tableQueryParams" ref="queryForm" labelWidth="90">
        <el-row>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10">
            <el-form-item label="Keywords">
              <el-input v-model="state.tableQueryParams.keyword" clearable placeholder="Please enter fuzzy search keywords"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="Name of the affiliated entity">
              <el-input v-model="state.tableQueryParams.entityName" clearable placeholder="Please enter the name of the entity you belong to"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="Associated Entity ID">
              <el-input v-model="state.tableQueryParams.entityId" clearable placeholder="Please enter the affiliated entity ID"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="Field Name">
              <el-input v-model="state.tableQueryParams.fieldName" clearable placeholder="Please enter a field name"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="Language code">
              <el-input v-model="state.tableQueryParams.langCode" clearable placeholder="Please enter language code"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="Translate content">
              <el-input v-model="state.tableQueryParams.content" clearable placeholder="Please enter the content to be translated"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10">
            <el-form-item >
              <el-button-group style="display: flex; align-items: center;">
                <el-button type="primary"  icon="ele-Search" @click="handleQuery" v-auth="'sysLangText:page'" v-reclick="1000"> Query </el-button>
                <el-button icon="ele-Refresh" @click="() => state.tableQueryParams = {}"> Reset </el-button>
                <el-button icon="ele-ZoomIn" @click="() => state.showAdvanceQueryUI = true" v-if="!state.showAdvanceQueryUI" style="margin-left:5px;"> Advanced query </el-button>
                <el-button icon="ele-ZoomOut" @click="() => state.showAdvanceQueryUI = false" v-if="state.showAdvanceQueryUI" style="margin-left:5px;"> hide </el-button>
                <el-button type="danger" style="margin-left:5px;" icon="ele-Delete" @click="batchDelSysLangText" :disabled="state.selectData.length == 0" v-auth="'sysLangText:batchDelete'"> Delete </el-button>
                <el-button type="primary" style="margin-left:5px;" icon="ele-Plus" @click="editDialogRef.openDialog(null, 'Add Translation')" v-auth="'sysLangText:add'"> Add New </el-button>
                <el-dropdown :show-timeout="70" :hide-timeout="50" @command="exportSysLangTextCommand">
                  <el-button type="primary" style="margin-left:5px;" icon="ele-FolderOpened" v-reclick="20000" v-auth="'sysLangText:export'"> Export </el-button>
                  <template #dropdown>
                    <el-dropdown-menu>
                      <el-dropdown-item command="select" :disabled="state.selectData.length == 0">Export Selected</el-dropdown-item>
                      <el-dropdown-item command="current">ExportThis page</el-dropdown-item>
                      <el-dropdown-item command="all">Export all</el-dropdown-item>
                    </el-dropdown-menu>
                  </template>
                </el-dropdown>
                <el-button type="warning" style="margin-left:5px;" icon="ele-MostlyCloudy" @click="importDataRef.openDialog()" v-auth="'sysLangText:import'"> import </el-button>
              </el-button-group>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </el-card>
    <el-card class="full-table" shadow="hover" style="margin-top: 5px">
      <el-table :data="state.tableData" @selection-change="(val: any[]) => { state.selectData = val; }" style="width: 100%" v-loading="state.tableLoading" tooltip-effect="light" row-key="id" @sort-change="sortChange" border>
        <el-table-column type="selection" width="40" align="center" v-if="auth('sysLangText:batchDelete') || auth('sysLangText:export')" />
        <el-table-column type="index" label="No" width="55" align="center"/>
        <el-table-column prop='entityName' label='Name of the affiliated entity' show-overflow-tooltip />
        <el-table-column prop='entityId' label='Associated Entity ID' show-overflow-tooltip />
        <el-table-column prop='fieldName' label='Field Name' show-overflow-tooltip />
        <el-table-column prop='langCode' label='Language code' show-overflow-tooltip />
        <el-table-column prop='content' label='Translate content' show-overflow-tooltip />
        <el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
          <template #default="scope">
            <ModifyRecord :data="scope.row" />
          </template>
        </el-table-column>
        <el-table-column label="Operation" width="140" align="center" fixed="right" show-overflow-tooltip v-if="auth('sysLangText:update') || auth('sysLangText:delete')">
          <template #default="scope">
            <el-button icon="ele-Edit" size="small" text type="primary" @click="editDialogRef.openDialog(scope.row, 'Edit translation')" v-auth="'sysLangText:update'"> Edit </el-button>
            <el-button icon="ele-Delete" size="small" text type="primary" @click="delSysLangText(scope.row)" v-auth="'sysLangText:delete'"> Delete </el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-pagination 
              v-model:currentPage="state.tableParams.page"
              v-model:page-size="state.tableParams.pageSize"
              @size-change="(val: any) => handleQuery({ pageSize: val })"
              @current-change="(val: any) => handleQuery({ page: val })"
              layout="total, sizes, prev, pager, next, jumper"
              :page-sizes="[10, 20, 50, 100, 200, 500]"
              :total="state.tableParams.total"
              size="small"
              background />
      <ImportData ref="importDataRef" :import="sysLangTextApi.importData" :download="sysLangTextApi.downloadTemplate" v-auth="'sysLangText:import'" @refresh="handleQuery"/>
      <printDialog ref="printDialogRef" :title="'Print translation'" @reloadTable="handleQuery" />
      <editDialog ref="editDialogRef" @reloadTable="handleQuery" />
    </el-card>
  </div>
</template>
<style scoped>
:deep(.el-input), :deep(.el-select), :deep(.el-input-number) {
  width: 100%;
}
</style>