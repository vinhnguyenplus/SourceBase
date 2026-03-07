<template>
    <div class="sys-tenant-container">
        <el-card shadow="hover" :body-style="{ padding: 5 }">
            <el-form :model="state.queryParams" ref="queryForm" :inline="true">
                <el-form-item label="Tenant name">
                    <el-input v-model="state.queryParams.name" placeholder="Tenant name" clearable />
                </el-form-item>
                <el-form-item label="Contact number">
                    <el-input v-model="state.queryParams.phone" placeholder="Contact number" clearable />
                </el-form-item>
                <el-form-item>
                    <el-button-group>
                        <el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysTenant:page'"> Query
                        </el-button>
                        <el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
                    </el-button-group>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="ele-Plus" @click="openAddTenant" v-auth="'sysTenant:add'"> Add New
                    </el-button>
                </el-form-item>
            </el-form>
        </el-card>

        <el-card class="full-table" shadow="hover" style="margin-top: 5px">
            <el-table :data="state.tenantData" style="width: 100%" :preserve-expanded-content="preserveExpanded" v-loading="state.loading" border>
                <el-table-column type="expand">
                    <template #default="props">
                        <el-descriptions :column="3" border>
                            <el-descriptions-item label="LOGO">
                                <div style="display: flex; align-items: center;"><el-avatar shape="square" :src="props.row.logo" size="small" /></div>
                            </el-descriptions-item>
                            <el-descriptions-item label="title">{{ props.row.title }}</el-descriptions-item>
                            <el-descriptions-item label="Subtitle">{{ props.row.viceTitle }}</el-descriptions-item>
                            
                            <el-descriptions-item label="domain name">{{ props.row.host }}</el-descriptions-item>
                            <el-descriptions-item label="Record Number">{{ props.row.icp }}</el-descriptions-item>
                            <el-descriptions-item label="Watermark">{{ props.row.watermark }}</el-descriptions-item>

                            <el-descriptions-item label="Database ID">{{ props.row.configId }}</el-descriptions-item>
                            <el-descriptions-item label="Copyright information" :span="2">{{ props.row.copyright }}</el-descriptions-item>
                            
                            <el-descriptions-item label="Database connection" :span="3">{{ props.row.connection }}</el-descriptions-item>
                            <el-descriptions-item label="Connect from library" :span="3">{{ props.row.slaveConnections }}</el-descriptions-item>
                        </el-descriptions>
                    </template>
                </el-table-column>
                <!-- <el-table-column type="index" label="No" width="55" align="center" fixed /> -->
                <!-- <el-table-column prop="logo" label="icon" width="55" align="center" show-overflow-tooltip>
                    <template #default="scope">
                        <el-avatar shape="square" :src="scope.row.logo" size="small" />
                    </template>
                </el-table-column> -->
                <el-table-column prop="name" label="name" width="180" align="center" show-overflow-tooltip />
                <!-- <el-table-column prop="title" label="title" width="180" show-overflow-tooltip /> -->
                <!-- <el-table-column prop="viceTitle" label="subtitle" width="180" show-overflow-tooltip />
                <el-table-column prop="viceDesc" label="Description" show-overflow-tooltip />
                <el-table-column prop="watermark" label="Watermark" width="130" show-overflow-tooltip />
                <el-table-column prop="copyright" label="Copyright information" width="350" show-overflow-tooltip />
                <el-table-column prop="icp" label="Record Number" width="130" show-overflow-tooltip />
                <el-table-column prop="icpUrl" label="icp address" width="280" show-overflow-tooltip />
                <el-table-column prop="enableReg" label="Enable registration" width="100" show-overflow-tooltip>
                    <template #default="scope">
                        <g-sys-dict v-model="scope.row.enableReg" code="YesNoEnum" />
                    </template>
                </el-table-column> -->
                <el-table-column prop="adminAccount" label="Rental account" align="center" width="120" show-overflow-tooltip />
                <el-table-column prop="phone" label="Telephone" width="150" align="center" show-overflow-tooltip />
                <el-table-column prop="host" label="domain name" width="200" show-overflow-tooltip />
                <!-- <el-table-column prop="email" label="Email" show-overflow-tooltip /> -->
                <el-table-column prop="tenantType" label="Tenant Type" width="100" align="center">
                    <template #default="scope">
                        <g-sys-dict v-model="scope.row.tenantType" code="TenantTypeEnum" />
                    </template>
                </el-table-column>
                <el-table-column label="state" width="70" align="center" class-name="status">
                    <template #default="scope">
                        <div>
                            <el-switch size="small" class="status-switch" 
                                v-model="scope.row.status"
                                :active-value="1"
                                :inactive-value="2" @change="changeStatus(scope.row)"
                                :disabled="scope.row.id == 123456780000000" 
                            />
                            <span class="status-tage">
                                <g-sys-dict v-model="scope.row.status" code="StatusEnum" />
                            </span>
                        </div>

                    </template>
                </el-table-column>
                <el-table-column prop="dbType" label="Database type" width="120" align="center">
                    <template #default="scope">
                        <el-tag v-if="scope.row.dbType === 0"> MySql </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 1"> SqlServer </el-tag>
                        <el-tag v-if="scope.row.dbType === 2"> Sqlite </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 3"> Oracle </el-tag>
                        <el-tag v-if="scope.row.dbType === 4"> PostgreSQL </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 5"> Dm </el-tag>
                        <el-tag v-if="scope.row.dbType === 6"> Kdbndp </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 7"> Oscar </el-tag>
                        <el-tag v-if="scope.row.dbType === 8"> MySqlConnector </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 9"> Access </el-tag>
                        <el-tag v-if="scope.row.dbType === 10"> OpenGauss </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 11"> QuestDB </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 12"> HG </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 13"> ClickHouse </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 14"> GBase </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 15"> Odbc </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 16"> OceanBaseForOracle </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 17"> TDengine </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 18"> GaussDB </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 19"> OceanBase </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 20"> Tidb </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 21"> Vastbase </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 22"> PolarDB </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 23"> Doris </el-tag>
                        <el-tag v-else-if="scope.row.dbType === 900"> Custom </el-tag>
                    </template>
                </el-table-column>
                <!-- <el-table-column prop="configId" label="Database ID" show-overflow-tooltip /> -->
                <!-- <el-table-column prop="connection" label="Database Connection" min-width="300" header-align="center" show-overflow-tooltip />
                <el-table-column prop="slaveConnections" label="Connect from library" min-width="300" header-align="center" show-overflow-tooltip /> -->
                <el-table-column prop="viceDesc" label="Description" show-overflow-tooltip />
                <el-table-column prop="orderNo" label="Sort" width="70" align="center" />
                <el-table-column label="Modify records" width="100" align="center">
                    <template #default="scope">
                        <ModifyRecord :data="scope.row" />
                    </template>
                </el-table-column>
                <el-table-column label="Operation" width="200" fixed="right" align="center">
                    <template #default="scope">
                        <el-button icon="ele-Coin" size="small" text type="danger" @click="createTenant(scope.row)"
                            v-auth="'sysTenant:createDb'" :disabled="scope.row.tenantType == 0"> Create Library </el-button>
                        <el-button icon="ele-Edit" size="small" text type="primary" @click="openEditTenant(scope.row)"
                            v-auth="'sysTenant:update'"> Edit </el-button>
                        <el-dropdown>
                            <el-button icon="ele-MoreFilled" size="small" text type="primary"
                                style="padding-left: 12px" />
                            <template #dropdown>
                                <el-dropdown-menu>
                                    <el-dropdown-item icon="ele-OfficeBuilding" @click="goTenant(scope.row)"
                                        :v-auth="'sysTenant:goTenant'"> Enter the rental management terminal </el-dropdown-item>
                                    <el-dropdown-item icon="ele-OfficeBuilding" @click="changeTenant(scope.row)"
                                        :v-auth="'sysTenant:changeTenant'"> Switch Tenant </el-dropdown-item>
                                    <el-dropdown-item icon="ele-OfficeBuilding" @click="openGrantMenu(scope.row)"
                                        :v-auth="'sysTenant:grantMenu'"> Authorization Menu </el-dropdown-item>
                                    <el-dropdown-item icon="ele-OfficeBuilding" @click="syncGrantMenu(scope.row)"
                                        :v-auth="'sysTenant:syncGrantMenu'" title="Used to synchronize authorization data after a version update"> Synchronous authorization
                                    </el-dropdown-item>
                                    <el-dropdown-item icon="ele-RefreshLeft" @click="resetTenantPwd(scope.row)"
                                        :v-auth="'sysTenant:resetPwd'"> reset password </el-dropdown-item>
                                    <el-dropdown-item icon="ele-Delete" @click="delTenant(scope.row)"
                                        :v-auth="'sysTenant:delete'"> Delete tenant </el-dropdown-item>
                                </el-dropdown-menu>
                            </template>
                        </el-dropdown>
                    </template>
                </el-table-column>
            </el-table>
            <el-pagination v-model:currentPage="state.tableParams.page" v-model:page-size="state.tableParams.pageSize"
                :total="state.tableParams.total" :page-sizes="[10, 20, 50, 100]" size="small" background
                @size-change="handleSizeChange" @current-change="handleCurrentChange"
                layout="total, sizes, prev, pager, next, jumper" />
        </el-card>

        <EditTenant ref="editTenantRef" :title="state.editTenantTitle" @handleQuery="handleQuery" />
        <GrantMenu ref="grantMenuRef" />
    </div>
</template>

<script lang="ts" setup name="sysTenant">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditTenant from '/@/views/system/tenant/component/editTenant.vue';
import GrantMenu from '/@/views/system/tenant/component/grantMenu.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysTenantApi } from '/@/api-services/api';
import { TenantOutput } from '/@/api-services/models';
import { reLoadLoginAccessToken } from "/@/utils/request";
import GSysDict from "/@/components/sysDict/sysDict.vue";

const preserveExpanded = ref(false)
const editTenantRef = ref<InstanceType<typeof EditTenant>>();
const grantMenuRef = ref<InstanceType<typeof GrantMenu>>();
const state = reactive({
    loading: false,
    tenantData: [] as Array<TenantOutput>,
    queryParams: {
        name: undefined,
        phone: undefined,
    },
    tableParams: {
        page: 1,
        pageSize: 50,
        total: 0 as any,
    },
    editTenantTitle: '',
});

onMounted(async () => {
    handleQuery();
});

// Query operation
const handleQuery = async () => {
    state.loading = true;
    let params = Object.assign(state.queryParams, state.tableParams);
    var res = await getAPI(SysTenantApi).apiSysTenantPagePost(params);
    state.tenantData = res.data.result?.items ?? [];
    state.tableParams.total = res.data.result?.total;
    state.loading = false;
};

// Enter the rental management terminal
const goTenant = (row: any) => {
    ElMessageBox.confirm(`Are you sure you want to enter the 【${row.name}】 rental management portal?`, 'Prompt', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
    }).then(() =>
        getAPI(SysTenantApi)
            .apiSysTenantGoTenantPost({ id: row.id })
            .then(res => reLoadLoginAccessToken(res.data.result))
    );
}

// Switch tenant
const changeTenant = (row: any) => {
    ElMessageBox.confirm(`Are you sure you want to switch the current user to [${row.name}]?`, 'Prompt', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
    }).then(() =>
        getAPI(SysTenantApi)
            .apiSysTenantChangeTenantPost({ id: row.id })
            .then(res => reLoadLoginAccessToken(res.data.result))
    );
}

const syncGrantMenu = (row: any) => {
    ElMessageBox.confirm(`Are you sure you want to synchronize the authorization data of [${row.name}]?`, 'Prompt', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
    }).then(async () => {
        await getAPI(SysTenantApi).apiSysTenantSyncGrantMenuPost({ id: row.id });
        ElMessage.success('Synchronous authorization successful');
    });
}

// reset operation
const resetQuery = () => {
    state.queryParams.name = undefined;
    state.queryParams.phone = undefined;
    handleQuery();
};

// Open new page
const openAddTenant = () => {
    state.editTenantTitle = 'Add Tenant';
    editTenantRef.value?.openDialog({ tenantType: 0, orderNo: 100, host: '' });
};

// Open the edit page
const openEditTenant = (row: any) => {
    state.editTenantTitle = 'Edit tenant';
    editTenantRef.value?.openDialog(row);
};

// Open the authorization menu page
const openGrantMenu = async (row: any) => {
    grantMenuRef.value?.openDialog(row);
};

// reset password
const resetTenantPwd = async (row: any) => {
    ElMessageBox.confirm(`Are you sure you want to reset the password for: 【${row.name}】?`, 'Prompt', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
    })
        .then(async () => {
            await getAPI(SysTenantApi)
                .apiSysTenantResetPwdPost({ userId: row.userId })
                .then((res) => {
                    ElMessage.success(`Password reset successfully: ${res.data.result}`);
                });
        })
        .catch(() => { });
};

// delete
const delTenant = (row: any) => {
    ElMessageBox.confirm(`Are you sure you want to delete the tenant: 【${row.name}】?`, 'Prompt', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
    })
        .then(async () => {
            await getAPI(SysTenantApi).apiSysTenantDeletePost({ id: row.id });
            handleQuery();
            ElMessage.success('Deleted successfully');
        })
        .catch(() => { });
};

// Change page capacity
const handleSizeChange = (val: number) => {
    state.tableParams.pageSize = val;
    handleQuery();
};

// Change page number
const handleCurrentChange = (val: number) => {
    state.tableParams.page = val;
    handleQuery();
};

// Create tenant library
const createTenant = (row: any) => {
    ElMessageBox.confirm(`Are you sure you want to create/update the tenant database: [${row.name}]?`, 'Prompt', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
    })
        .then(async () => {
            await getAPI(SysTenantApi).apiSysTenantCreateDbPost({ id: row.id });
            ElMessage.success('Create/update tenant database successfully');
        })
        .catch(() => { });
};

// Modify status
const changeStatus = (row: any) => {
    getAPI(SysTenantApi)
        .apiSysTenantSetStatusPost({ id: row.id, status: row.status })
        .then(() => {
            ElMessage.success('Tenant status set successfully');
        })
        .catch(() => {
            row.status = row.status == 1 ? 2 : 1;
        });
};
</script>

<style lang="scss" scoped>
.status {
    width: 100%;

    .status-switch {
        display: none;
        height: 100%;
        line-height: 100%;
    }

    .status-tage {
        display: block;
    }
}
.status:hover {
    .status-switch {
        display: block;
    }

    .status-tage {
        display: none;
    }
}

:deep(.el-table__expanded-cell) {
    padding: 10px 45px !important;
    .el-descriptions__body {
        .el-descriptions__table { table-layout: fixed; }
        .el-descriptions__label { width: 150px; }
    }
}
</style>