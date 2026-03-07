<script lang="ts" setup name="sysLang">
import { ref, reactive, onMounted } from "vue";
import { auth } from '/@/utils/authFunction';
import { ElMessageBox, ElMessage } from "element-plus";
import { getAPI } from '/@/utils/axios-utils';
import { SysLangApi } from '/@/api-services/api';
import editDialog from '/@/views/system/lang/component/editDialog.vue'
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import { SysLangOutput } from '/@/api-services/models/sys-lang-output';

const editDialogRef = ref();
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
    field: 'active', // Default sort field
    order: 'descending', // Sorting direction
    descStr: 'descending', // Key characters for sorting in descending order
  },
  tableData: [] as SysLangOutput[],
});

// When the page loads
onMounted(async () => {
});

// Query operation
const handleQuery = async (params: any = {}) => {
  state.tableLoading = true;
  state.tableParams = Object.assign(state.tableParams, params);
  const result = await  getAPI(SysLangApi).apiSysLangPagePost(Object.assign(state.tableQueryParams, state.tableParams)).then(res => res.data.result);
  state.tableParams.total = result?.total ?? 0;
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
const delSysLang = (row: any) => {
  ElMessageBox.confirm(`ConfirmwantDelete??`, "Prompt", {
    confirmButtonText: "Confirm",
    cancelButtonText: "Cancel",
    type: "warning",
  }).then(async () => {
    await getAPI(SysLangApi).apiSysLangDeletePost({ id: row.id });
    handleQuery();
    ElMessage.success("Deleted successfully");
  }).catch(() => {});
};

const batchDelSysLang = () => {
  ElMessageBox.confirm(`ConfirmwantDeleteSelectinof ${state.selectData.length} record??`, "Prompt", {
    confirmButtonText: "Confirm",
    cancelButtonText: "Cancel",
    type: "warning",
  }).then(async () => {
    const ids = state.selectData.map((item) => item.id);
    //await getAPI(SysLangApi).apiSysLangBatchDeletePost({ ids });
    state.selectData = [];
    handleQuery();
    ElMessage.success("Deleted successfully");
  }).catch(() => {});
};

handleQuery();
</script>
<template>
  <div class="sysLang-container" v-loading="state.exportLoading">
    <el-card shadow="hover" :body-style="{ paddingBottom: '0' }"> 
      <el-form :model="state.tableQueryParams" ref="queryForm" labelWidth="90">
        <el-row>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10">
            <el-form-item label="Keywords">
              <el-input v-model="state.tableQueryParams.keyword" clearable placeholder="Please enter fuzzy search keywords"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="Language name">
              <el-input v-model="state.tableQueryParams.name" clearable placeholder="Please enter language name"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="Language code">
              <el-input v-model="state.tableQueryParams.code" clearable placeholder="Please enter language code"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="ISO language code">
              <el-input v-model="state.tableQueryParams.isoCode" clearable placeholder="Please enter the ISO language code"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="URL language code">
              <el-input v-model="state.tableQueryParams.urlCode" clearable placeholder="Please enter the URL language code"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="state.showAdvanceQueryUI">
            <el-form-item label="Enable or not">
              <el-input v-model="state.tableQueryParams.active" clearable placeholder="Please enter whether to enable"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10">
            <el-form-item >
              <el-button-group style="display: flex; align-items: center;">
                <el-button type="primary"  icon="ele-Search" @click="handleQuery" v-auth="'sysLang:page'" v-reclick="1000"> Query </el-button>
                <el-button type="danger" style="margin-left:5px;" icon="ele-Delete" @click="batchDelSysLang" :disabled="state.selectData.length == 0" v-auth="'sysLang:batchDelete'"> Delete </el-button>
                <el-button type="primary" style="margin-left:5px;" icon="ele-Plus" @click="editDialogRef.openDialog(null, 'Add New Multilanguage')" v-auth="'sysLang:add'"> Add New </el-button>
              </el-button-group>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </el-card>
    <el-card class="full-table" shadow="hover" style="margin-top: 5px">
      <el-table :data="state.tableData" style="width: 100%" v-loading="state.tableLoading" tooltip-effect="light" row-key="id" @sort-change="sortChange" border>
        <el-table-column type="index" label="No" width="55" align="center"/>
        <el-table-column prop='name' label='Language name' sortable='custom' show-overflow-tooltip />
        <el-table-column prop='code' label='Language code' sortable='custom' show-overflow-tooltip />
        <el-table-column prop='isoCode' label='ISO language code' sortable='custom' show-overflow-tooltip />
        <el-table-column prop='urlCode' label='URL language code' sortable='custom' show-overflow-tooltip />
        <el-table-column prop='direction' label='Writing direction' sortable='custom' show-overflow-tooltip  >
          <template #default="scope">
						<g-sys-dict v-model="scope.row.direction" code="DirectionEnum" />
					</template>
        </el-table-column>
        <el-table-column prop='dateFormat' label='date format' sortable='custom' show-overflow-tooltip />
        <el-table-column prop='timeFormat' label='time format' sortable='custom' show-overflow-tooltip />
        <el-table-column prop='weekStart' label='Start day of the week' sortable='custom' show-overflow-tooltip >
          <template #default="scope">
						<g-sys-dict v-model="scope.row.weekStart" code="WeekEnum" />
					</template>
        </el-table-column>
        <el-table-column prop='grouping' label='Grouping symbols' sortable='custom' show-overflow-tooltip />
        <el-table-column prop='decimalPoint' label='Decimal point symbol' sortable='custom' show-overflow-tooltip />
        <el-table-column prop='thousandsSep' label='thousands separator' sortable='custom' show-overflow-tooltip />
        <el-table-column prop='active' label='Enable or not' sortable='custom' show-overflow-tooltip>
          <template #default="scope">
            <el-tag v-if="scope.row.active"> Yes </el-tag>
            <el-tag type="danger" v-else> no </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="Modify records" width="100" align="center" show-overflow-tooltip>
          <template #default="scope">
            <ModifyRecord :data="scope.row" />
          </template>
        </el-table-column>
        <el-table-column label="Operation" width="140" align="center" fixed="right" show-overflow-tooltip v-if="auth('sysLang:update') || auth('sysLang:delete')">
          <template #default="scope">
            <el-button icon="ele-Edit" size="small" text type="primary" @click="editDialogRef.openDialog(scope.row, 'Edit Multilingual')" v-auth="'sysLang:update'"> Edit </el-button>
            <el-button icon="ele-Delete" size="small" text type="primary" @click="delSysLang(scope.row)" v-auth="'sysLang:delete'"> Delete </el-button>
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
      <editDialog ref="editDialogRef" @reloadTable="handleQuery" />
    </el-card>
  </div>
</template>
<style scoped>
:deep(.el-input), :deep(.el-select), :deep(.el-input-number) {
  width: 100%;
}
</style>