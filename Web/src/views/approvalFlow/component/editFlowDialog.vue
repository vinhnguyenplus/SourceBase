<template>
	<div class="flow-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" fullscreen>
			<template #header>
				<div style="color: #fff">
					<span>{{ props.title }}</span>
				</div>
			</template>
			<div class="f-content">
				<div class="f-container">
					<div class="f-switch">
						<el-switch v-model="state.value2" @change="change" class="mb-2" active-text="Open marquee selection" inactive-text="Turn off selection box" />
					</div>
					<PanelControl v-if="lf" :lf="lf" @catData="getData"></PanelControl>
					<div class="f-container-c" ref="container" id="container"></div>
					<PanelNode v-if="lf" :lf="lf"></PanelNode>
					<el-drawer title="Attribute" v-model="drawer" :direction="direction" size="500px" :before-close="handleClose">
						<PropertyDialog v-if="drawer" :nodeData="state.nodeData" :lf="lf" @setPropertiesFinish="handleClose"></PropertyDialog>
					</el-drawer>
					<el-dialog title="Data" v-model="dataVisible" width="50%">
						<PanelDataDialog :graphData="state.graphData"></PanelDataDialog>
					</el-dialog>
				</div>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">Cancel</el-button>
					<el-button type="primary" @click="submit">Confirm</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { reactive, ref, nextTick } from 'vue';
import { ElMessageBox } from 'element-plus';

import LogicFlow from '@logicflow/core';
import { BpmnElement, InsertNodeInPolyline, Menu, MiniMap, SelectionSelect, Snapshot } from '@logicflow/extension';
import '@logicflow/core/dist/index.css';
import '@logicflow/extension/lib/style/index.css';

import RegisterEdge from './LogicFlow/Register/RegisterEdge';
import RegisterNode from './LogicFlow/Register/RegisterNode';
import PanelNode from './LogicFlow/Panel/PanelNode.vue';
import PanelControl from './LogicFlow/Panel/PanelControl.vue';
import PanelDataDialog from './LogicFlow/Panel/PanelDataDialog.vue';
import PropertyDialog from './LogicFlow/Property/PropertyDialog.vue';

import { getAPI } from '/@/utils/axios-utils';
import { ApprovalFlowApi } from '/@/api-plugins/approvalFlow/api';
import { ApprovalFlowOutput, UpdateApprovalFlowInput } from '/@/api-plugins/approvalFlow/models';

var props = defineProps({
	title: {
		type: String,
		default: '',
	},
});

const emit = defineEmits(['reloadTable', 'updateFlow']);
const flowData = ref({});
const lf = ref<InstanceType<typeof LogicFlow>>();

const drawer = ref(false);
const direction = ref('rtl');
const dataVisible = ref(false);

const state = reactive({
	loading: false,
	isShowDialog: false,
	ruleSource: {} as UpdateApprovalFlowInput,
	nodeData: {},
	graphData: {},
});

const openDialog = (row: ApprovalFlowOutput) => {
	state.ruleSource = row as UpdateApprovalFlowInput;
	// initialization data
	if (state.ruleSource.flowJson) {
		flowData.value = JSON.parse(state.ruleSource.flowJson);
	} else {
		flowData.value = {
			nodes: [],
			edges: [],
		};
	}
	state.isShowDialog = true;
	nextTick(() => {
		// Initialize canvas
		initGraph();
	});
	console.log('open');
};

const closeDialog = () => {
	emit('reloadTable');
	state.isShowDialog = false;
	console.log('close');
};

const cancel = () => {
	state.isShowDialog = false;
	console.log('cancel');
};

// Save process design
const submit = async () => {
	flowData.value = lf.value?.getGraphData();
	state.ruleSource.flowJson = JSON.stringify(flowData.value);
	await getAPI(ApprovalFlowApi).apiApprovalFlowUpdatePost(state.ruleSource);
	emit('updateFlow', flowData.value);
	closeDialog();
};

const initGraph = () => {
	// Initialize canvas
	const container: HTMLElement = document.querySelector('#container')!;
	// Configuration items
	const config = {
		stopScrollGraph: true, // Disable mouse scrolling to move canvas
		stopZoomGraph: true, // Disable zoom
		metaKeyMultipleSelected: true,
		// Background grid size
		grid: {
			size: 10,
			type: 'dot',
		},
		// shortcut key
		keyboard: {
			enabled: true,
		},
		// auxiliary line
		snapline: true,
	};
	lf.value = new LogicFlow({
		...config,
		plugins: [
			BpmnElement,
			// Zuodong nodes automatically insert edges
			InsertNodeInPolyline,
			// right click menu
			Menu,
			// Sparklines
			MiniMap,
			// Frame selection
			SelectionSelect,
			// Snapshot
			Snapshot,
		],
		container: container,
		width: container.clientWidth,
		height: container.clientHeight,
	});
	// Set theme
	lf.value.setTheme({
		snapline: {
			stroke: '#1E90FF', // Alignment line color
			strokeWidth: 1, // alignment line width
		},
	});
	// Register a custom node
	RegisterNode.Register(lf.value);
	// Register a custom edge
	RegisterEdge.Register(lf.value);
	// Listen to node click events
	lf.value.on('node:click', ({ data }) => {
		state.nodeData = data;
		drawer.value = true;
	});
	// Listen for edge click events
	lf.value.on('edge:click', ({ data }) => {
		state.nodeData = data;
		drawer.value = true;
	});
	// render data
	lf.value.render(flowData.value);
	// Center canvas
	lf.value.focusOn({ coordinate: { x: 300, y: 300 } });
};

// Frame selection
const change = (val: boolean) => {
	if (val) {
		lf.value?.extension.selectionSelect.openSelectionSelect();
	} else {
		lf.value?.extension.selectionSelect.closeSelectionSelect();
	}
};

// Get data
const getData = () => {
	var data = lf.value?.getGraphData();
	state.graphData = data;
	dataVisible.value = true;
};

// Close property interface reminder
const handleClose = (done: () => void) => {
	ElMessageBox.confirm('Are you sure you want to close current property editing?')
		.then(() => {
			done();
		})
		.catch(() => {
			// catch error
		});
};

defineExpose({ openDialog });
</script>

<style scoped lang="scss">
:deep(.el-tabs__nav-scroll) {
	width: 70%;
	margin: 0 auto;
}
.flow-container {
	:deep(.el-dialog) {
		.el-dialog__header {
			display: none !important;
		}
		.el-dialog__body {
			max-height: calc(100vh - 45px) !important;
		}
	}
}
.f-content {
	display: flex;
	flex-grow: 1;
	z-index: 1;
	margin: -2px -19px -20px -19px;
	height: calc(100vh - 100px) !important;

	.f-container {
		flex-grow: 1;
		position: relative;

		.f-switch {
			position: absolute;
			z-index: 2;
			top: -22px;
			left: 5px;

			.el-switch {
				margin-right: 10px;
			}
		}

		.el-drawer {
			height: 80%;
			overflow: auto;
			margin-top: -30px;
			z-index: !important;
		}

		.f-container-c {
			position: absolute;
			width: 100%;
			height: 100%;
		}
	}
}
</style>
