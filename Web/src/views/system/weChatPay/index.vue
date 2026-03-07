<template>
	<div class="weChatPay-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="Order Number">
					<el-input v-model="state.queryParams.keyword" clearable placeholder="Please enter the order number" />
				</el-form-item>
				<el-form-item label="Creation Time">
					<el-date-picker placeholder="Please select creation time" value-format="YYYY/MM/DD" type="daterange" v-model="state.queryParams.createTimeRange" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery"> Query </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> reset </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddDialog">Add simulation data</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="outTradeNumber" label="Merchant Order Number" width="180"></el-table-column>
				<el-table-column prop="transactionId" label="Payment Order Number" width="220"></el-table-column>
				<el-table-column prop="description" label="Description" width="180"></el-table-column>
				<el-table-column prop="total" :formatter="amountFormatter" label="Amount" width="70"></el-table-column>
				<el-table-column prop="tradeState" label="state" width="70">
					<template #default="scope">
						<el-tag v-if="scope.row.tradeState == 'SUCCESS'" type="success"> Completed </el-tag>
						<el-tag v-else-if="scope.row.tradeState == 'REFUND'" type="danger"> Refund </el-tag>
						<el-tag v-else type="info"> Incomplete </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="attachment" label="Additional Information" width="180"></el-table-column>
				<el-table-column prop="tags" label="BusinessType" width="90"></el-table-column>
				<el-table-column prop="createTime" label="Creation Time" width="150"></el-table-column>
				<el-table-column prop="successTime" label="completion time" width="150"></el-table-column>
				<el-table-column prop="businessId" label="Business ID" width="130"></el-table-column>
				<el-table-column label="Operation" align="center" fixed="right">
					<template #default="scope">
						<el-button
              text
							size="small"
							type="primary"
							v-if="scope.row.qrcodeContent != null && scope.row.qrcodeContent != '' && (scope.row.tradeState === '' || !scope.row.tradeState)"
							@click="openQrDialog(scope.row.qrcodeContent)"
							>Payment QR code</el-button
						>
						<el-button size="small" text type="primary" v-if="scope.row.tradeState === 'REFUND'" @click="openRefundDialog(scope.row.transactionId)">View refund</el-button>
						<el-button size="small" text type="primary" v-if="scope.row.tradeState === 'SUCCESS'" @click="doRefund(scope.row)">Full refund</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage="state.tableParams.page"
				v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total"
				:page-sizes="[10, 20, 50, 100]"
				size="small"
				background
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>

		<el-dialog v-model="showAddDialog">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>Add simulation data</span>
				</div>
			</template>
			<el-form>
				<el-form-item label="Product">
					<el-input v-model="addData.description" placeholder="Required" clearable />
				</el-form-item>
				<el-form-item label="Amount(points)">
					<el-input v-model="addData.total" placeholder="Required, fill in the number, the unit is minutes" clearable />
				</el-form-item>
				<el-form-item label="Additional Information">
					<el-input v-model="addData.attachment" clearable />
				</el-form-item>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="closeAddDialog">Cancel</el-button>
					<el-button type="primary" @click="saveData">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
		<el-dialog v-model="showQrDialog">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-View /> </el-icon>
					<span>Payment QR code</span>
				</div>
			</template>
			<div ref="qrDiv"></div>
		</el-dialog>

		<el-dialog v-model="showRefundDialog">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Document /> </el-icon>
					<span>Refund Information</span>
				</div>
			</template>
			<el-table :data="subTableData" style="width: 100%" tooltip-effect="light" row-key="id" border>
				<el-table-column type="index" label="No" width="55" align="center" />
				<el-table-column prop="outRefundNumber" label="Merchant refund number" width="180"></el-table-column>
				<el-table-column prop="transactionId" label="Payment Order Number" width="220"></el-table-column>
				<el-table-column prop="refund" label="Amount(points)" width="70"></el-table-column>
				<el-table-column prop="reason" label="Reason for refund" width="180"></el-table-column>
				<el-table-column prop="tradeState" label="state" width="70">
					<template #default="scope">
						<el-tag v-if="scope.row.tradeState == 'SUCCESS'" type="success"> Completed </el-tag>
						<el-tag v-else-if="scope.row.tradeState == 'REFUND'" type="danger"> Refund </el-tag>
						<el-tag v-else type="info"> Incomplete </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="remark" label="Remarks" width="180"></el-table-column>
				<el-table-column prop="createTime" label="Creation Time" width="150"></el-table-column>
				<el-table-column prop="successTime" label="completion time" width="150"></el-table-column>
			</el-table>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="sysWechatPay">
import { ref, nextTick, onMounted, reactive } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import QRCode from 'qrcodejs2-fixes';
import { pagePayList, createPay, getRefundListByID, refundDomestic } from '/@/api/system/weChatPay';
import { SysWechatPay } from '/@/api-services/models';

const qrDiv = ref<HTMLElement | null>(null);
const showAddDialog = ref(false);
const showQrDialog = ref(false);
const showRefundDialog = ref(false);

const subTableData = ref<any>([]);
const addData = ref<any>({});

const state = reactive({
	loading: false,
	tableData: [] as Array<SysWechatPay>,
	queryParams: {
		keyword: undefined,
		createTimeRange: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editTenantTitle: '',
});

// Page initialization
onMounted(async () => {
	handleQuery();
});

// Query operation
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await pagePayList(params);
	let tmpRows = res.data.result?.items ?? [];
	state.tableData = tmpRows;
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// reset operation
const resetQuery = () => {
	state.queryParams.keyword = undefined;
	state.queryParams.createTimeRange = undefined;
	handleQuery();
};

// Refund
const doRefund = async (orderInfo: any) => {
	ElMessageBox.prompt(`Confirm refund: ${orderInfo.total / 100} yuan? Please enter the reason for the refund`, 'Prompt', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
	})
		.then(async ({ value }) => {
			let resp = await refundDomestic({
				tradeId: orderInfo.outTradeNumber,
				reason: value,
				refund: orderInfo.total,
				total: orderInfo.total,
			});
			if (resp.data.code == 200) {
				ElMessage.success(`【${value}】Refund application successful`);
			} else {
				ElMessage.error('Operation failed:' + resp.data.message);
			}
		})
		.catch(() => {
			ElMessage.error('Cancel operation');
		});
};

const amountFormatter = (row: any, column: any, cellValue: number, index: number) => {
	return (cellValue / 100).toFixed(2);
};

// Open new page
const openAddDialog = () => {
	addData.value = {
		description: null,
		total: null,
		attachment: null,
	};
	showAddDialog.value = true;
};

// Close the new page
const closeAddDialog = () => {
	showAddDialog.value = false;
};

// Open the code scanning page
const openQrDialog = (code: string) => {
	showQrDialog.value = true;
	nextTick(() => {
		(<HTMLElement>qrDiv.value).innerHTML = '';
		new QRCode(qrDiv.value, {
			text: code,
			width: 260,
			height: 260,
			colorDark: '#000000',
			colorLight: '#ffffff',
		});
	});
};

// Open the refund page
const openRefundDialog = async (code: string) => {
	var res = await getRefundListByID(code);
	if (res.data.code === 200) {
		let tmpRows = res.data.result ?? [];
		subTableData.value = tmpRows;
		showRefundDialog.value = true;
	} else {
		ElMessage.error('Failed to obtain refund list,' + res.data.message);
	}
};

// save data
const saveData = async () => {
	var res = await createPay(addData.value);
	if (res.data.code === 200) {
		closeAddDialog();
		let code = res.data.result.qrcodeUrl;
		openQrDialog(code);
		handleQuery();
	} else {
		ElMessage.error('Creation failed,' + res.data.message);
	}
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
</script>
