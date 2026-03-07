<template>
	<el-row :gutter="8" style="margin-bottom: 10px">
		<!-- Process drop-down template selection -->
		<el-col :span="4">
			<el-select v-model="mode" showSearch @change="changeMode" :defaultValue="0" option-label-prop="label" class="w100">
				<el-option v-for="(opt, idx) in state.modeList" :key="idx" :label="opt.name" :value="idx">
					{{ opt.name }}
				</el-option>
			</el-select>
		</el-col>

		<el-col :span="20">
			<el-select v-model="state.curPaper.type" placeholder="Default paper" style="width: 120px" @change="setPaper">
				<el-option v-for="item in state.paperTypes" :key="item.type" :label="item.type" :value="item.type" />
			</el-select>
			<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />
			<!-- Paper settings -->
			<el-button-group>
				<el-popover v-model="state.paperPopVisible" placement="bottom" width="300" title="Set paper width and height (mm)">
					<div style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 10px">
						<el-input type="number" v-model="state.paperWidth" style="width: 100px; text-align: center" place="Width (mm)"></el-input>~
						<el-input type="number" v-model="state.paperHeight" style="width: 100px; text-align: center" place="Height (mm)"></el-input>
					</div>
					<div>
						<el-button type="primary" style="width: 100%" @click="otherPaper">Confirm</el-button>
					</div>
					<template #reference>
						<el-button :type="'other' == curPaperType ? 'primary' : ''">Custom width and height</el-button>
					</template>
				</el-popover>
			</el-button-group>
			<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />
			<el-input-number style="margin-left: 5px; width: 130px" v-model="state.scaleValue" :precision="2" :step="0.1" :min="state.scaleMin" :max="state.scaleMax" @change="changeScale"></el-input-number>
			<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />
			<el-button-group>
				<el-tooltip content="left aligned" placement="bottom">
					<el-button icon="ele-Back" @click="setElsAlign('left')"> </el-button>
				</el-tooltip>
				<el-tooltip content="center" placement="bottom">
					<el-button icon="ele-FullScreen" @click="setElsAlign('vertical')"> </el-button>
				</el-tooltip>
				<el-tooltip content="Align right" placement="bottom">
					<el-button icon="ele-Right" @click="setElsAlign('right')"> </el-button>
				</el-tooltip>
				<el-tooltip content="top aligned" placement="bottom">
					<el-button icon="ele-Top" @click="setElsAlign('top')"> </el-button>
				</el-tooltip>
				<el-tooltip content="verticalcenter" placement="bottom">
					<el-button icon="ele-DCaret" @click="setElsAlign('horizontal')"> </el-button>
				</el-tooltip>
				<el-tooltip content="bottom aligned" placement="bottom">
					<el-button icon="ele-Bottom" @click="setElsAlign('bottom')"> </el-button>
				</el-tooltip>
				<el-tooltip content="Horizontal dispersion" placement="bottom">
					<el-button icon="ele-Sort" @click="setElsAlign('distributeHor')"> </el-button>
				</el-tooltip>
				<el-tooltip content="Vertical dispersion" placement="bottom">
					<el-button icon="ele-Switch" @click="setElsAlign('distributeVer')"> </el-button>
				</el-tooltip>
			</el-button-group>
			<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />
			<el-button-group>
				<el-tooltip content="rotate" placement="bottom">
					<el-button icon="ele-RefreshRight" @click="rotatePaper"></el-button>
				</el-tooltip>
				<el-tooltip content="Preview" placement="bottom">
					<el-button icon="ele-View" @click="preView"></el-button>
				</el-tooltip>
				<el-tooltip content="Clear template" placement="bottom">
					<el-button icon="ele-Delete" @click="clearPaper"></el-button>
				</el-tooltip>
				<el-tooltip content="Print directly" placement="bottom">
					<el-button icon="ele-Printer" @click="print"> </el-button>
				</el-tooltip>
				<el-tooltip content="TemplateJSON" placement="bottom">
					<el-button icon="ele-Coin" @click="viewJson"> </el-button>
				</el-tooltip>
			</el-button-group>
		</el-col>
	</el-row>

	<el-row :gutter="8" style="height: calc(100% - 42px);">
		<el-col :span="4">
			<CardPro shadow="never" full-height :body-style="{overflow: 'auto'}">
                <div id="hiprintEpContainer" class="rect-printElement-types hiprintEpContainer"></div>
			</CardPro>
		</el-col>
		<el-col :span="14" style="height: 100%;">
			<CardPro shadow="never" full-height :body-style="{overflow: 'auto'}">
                <div id="hiprint-printTemplate" class="hiprint-printTemplate"></div>
			</CardPro>
		</el-col>
		<el-col :span="6" class="params_setting_container" style="height: 100%;">
			<el-tabs type="border-card" style="height: 100%; overflow: auto;">
				<el-tab-pane label="Attribute" style="height: 100%;">
                    <!-- <CardPro full-height shadow="never"> -->
						<el-row class="hinnn-layout-sider">
							<div id="PrintElementOptionSetting"></div>
						</el-row>
					<!-- </CardPro> -->
				</el-tab-pane>
				<el-tab-pane label="Test data">
					<el-button @click="formatPrintDataDemo()" style="margin-bottom: 10px; width: 100%">Formatted string</el-button>
					<el-input v-model="printDataDemo" type="textarea" style="width: 100%" :rows="30" placeholder="Complete test data for the entire document"></el-input>
				</el-tab-pane>
			</el-tabs>
		</el-col>
	</el-row>

	<el-drawer title="Print template" v-model="state.templateDialogVisible">
		<vue-json-pretty :data="state.templateContent" showLength showIcon showLineNumber showSelectController />
	</el-drawer>

	<!-- Preview -->
	<PrintPreview ref="preViewRef" title="Preview" />
</template>

<script lang="ts" setup name="hiprintDesign">
import { computed, onMounted, ref, reactive } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';

import { hiprint } from 'vue-plugin-hiprint';
import providers from './providers';
import PrintPreview from './preview.vue';
import printDataDefault from './print-data';
import CardPro from '/@/components/CardPro/index.vue';
// import { IPaperType } from './type';

interface IPaperType {
	type: string;
	width: number;
	height: number;
}

var props = defineProps({
	modeIndex: {
		type: Number,
		default: 0,
	},
});

let hiprintTemplate = ref();
let mode = ref(0); // Template selection

const preViewRef = ref();
const printDataDemo = ref('');
const state = reactive({
	modeList: [] as any,
	// Current paper
	curPaper: {
		type: 'A4',
		width: 220,
		height: 296.6,
	} as IPaperType,
	// Paper type
	paperTypes: [
		{
			type: 'A3',
			width: 420,
			height: 296.6,
		},
		{
			type: 'A4',
			width: 210,
			height: 296.6,
		},
		{
			type: 'A5',
			width: 210,
			height: 147.6,
		},
		{
			type: 'B3',
			width: 500,
			height: 352.6,
		},
		{
			type: 'B4',
			width: 250,
			height: 352.6,
		},
		{
			type: 'B5',
			width: 250,
			height: 175.6,
		},
		{
			type: '4R',
			width: 152,
			height: 102,
		},
		{
			type: '6R',
			width: 203,
			height: 152,
		},
	] as IPaperType[],
	scaleValue: 1,
	scaleMax: 5,
	scaleMin: 0.5,
	// Custom paper
	paperPopVisible: false,
	paperWidth: 220,
	paperHeight: 80,

	templateDialogVisible: false,
	templateContent: '',
});

// Calculate current paper type
const curPaperType = computed(() => {
	let { width, height } = state.curPaper;
	let type = 'other';
	let types: any = state.paperTypes;
	for (const key in types) {
		let item = types[key];
		if (item.width === width && item.height === height) {
			type = key;
			break;
		}
	}
	return type;
});

// Select template
const changeMode = () => {
	let provider = providers[mode.value];
	hiprint.init({
		providers: [provider.f],
	});
	// Rendering customization options
	const hiprintEpContainerEl = document.getElementById('hiprintEpContainer');
	if (hiprintEpContainerEl) {
		hiprintEpContainerEl.innerHTML = '';
	}
	hiprint.PrintElementTypeManager.build('.hiprintEpContainer', provider.value);

	// Render painting template
	const hiprintPrintTemplate = document.getElementById('hiprint-printTemplate');
	if (hiprintPrintTemplate) {
		hiprintPrintTemplate.innerHTML = '';
	}
	// Initialize the print template designer
	let template = {};
	hiprintTemplate.value = new hiprint.PrintTemplate({
		template: template,
		settingContainer: '#PrintElementOptionSetting',
		paginationContainer: '.hiprint-printPagination',
		fontList: [
			{ title: 'Microsoft YaHei', value: 'Microsoft YaHei' },
			{ title: 'black body', value: 'STHeitiSC-Light' },
			{ title: 'Arial', value: 'Arial' },
			{ title: 'Song Dynasty', value: 'SimSun' },
			{ title: 'Huawei regular script', value: 'STKaiti' },
			{ title: 'cursive', value: 'cursive' },
			{ title: 'Vector', value: 'Vector' },
		],
	});
	hiprintTemplate.value.design('#hiprint-printTemplate');
	// Get the current magnification ratio, which will only be available when true is passed when zooming.
	state.scaleValue = hiprintTemplate.value.editingPanel?.scale ?? 1;
};

/**
 * Set paper size
 * @param type [A3, A4, A5, B3, B4, B5, other]
 * @param value {width,height} mm
 */
const setPaper = (type: string, value?: { width: number; height: number }) => {
	try {
		const paperType = state.paperTypes.find((x) => x.type == type);
		if (paperType) {
			state.curPaper = { type: type, width: paperType.width || 0, height: paperType.height || 0 };
			hiprintTemplate.value.setPaper(paperType.width, paperType.height);
		} else {
			state.curPaper = { type: 'other', width: value?.width || 0, height: value?.height || 0 };
			hiprintTemplate.value.setPaper(value?.width, value?.height);
		}
	} catch (error) {
		ElMessage.error(`Operation failed: ${error}`);
	}
};

// Change zoom ratio
const changeScale = (currentValue: number, oldValue: number) => {
	let big = false;
	currentValue <= oldValue ? (big = false) : (big = true);

	let scaleVal = currentValue;
	if (big) {
		if (scaleVal > state.scaleMax) scaleVal = 5;
	} else {
		if (scaleVal < state.scaleMin) scaleVal = 0.5;
	}
	if (hiprintTemplate.value) {
		// scaleVal: enlarge or reduce the value, false: do not save (the same is true if it is not passed), if true is passed, it will also be enlarged when printing.
		hiprintTemplate.value.zoom(scaleVal);
		state.scaleValue = scaleVal;
	}
};

// Rotate template
const rotatePaper = () => {
	if (hiprintTemplate.value) {
		hiprintTemplate.value.rotatePaper();
	}
};

// Align template
const setElsAlign = (e: any) => {
	hiprintTemplate.value.setElsAlign(e);
};

// Clear template
const clearPaper = () => {
	ElMessageBox.confirm('Are you sure to clear the template information?', 'warning', {
		confirmButtonText: 'Confirm',
		cancelButtonText: 'Cancel',
		type: 'warning',
	})
		.then(() => {
			try {
				hiprintTemplate.value.clear();
			} catch (error) {
				ElMessage.error(`Operation failed: ${error}`);
			}
		})
		.catch((err) => {
			console.log(err);
		});
};

// Custom paper
const otherPaper = () => {
	let value = {
		width: 0,
		height: 0,
	};
	value.width = state.paperWidth;
	value.height = state.paperHeight;
	state.paperPopVisible = false;
	setPaper('other', value);
};

// Preview
const preView = () => {
	let { width } = state.curPaper;
	let printData = null;
	try {
		printData = JSON.parse(printDataDemo.value);
	} catch (e) {
		console.log('Error:' + e);
	}
	if (printData == null) {
		printData = printDataDefault;
	}
	preViewRef.value.showDialog(hiprintTemplate.value, printData, width);
};
// Print directly
const print = () => {
	preView();
};

// View template JSON
const viewJson = () => {
	if (hiprintTemplate.value) {
		var templateJson = JSON.stringify(hiprintTemplate.value.getJson() || {});
		state.templateContent = JSON.parse(templateJson);
		state.templateDialogVisible = true;
	}
};

onMounted(() => {
	state.modeList = providers.map((e) => {
		return { type: e.type, name: e.name, value: e.value };
	});
	mode.value = props.modeIndex;
	changeMode();
	// otherPaper(); //Default paper
});

// Initialize paper size
const initPaper = () => {
	var template = hiprintTemplate.value.getJson();
	var width = template.panels[0].width;
	var height = template.panels[0].height;
	const paperType = state.paperTypes.find((x) => x.width == width && x.height == height);
	state.curPaper = { type: paperType?.type || '', width: width, height: height }; // Calculate paper type and status
	hiprintTemplate.value.setPaper(width, height); // Set paper size
};

// Set up preview test data
const setPrintDataDemo = (strData: string | null | undefined) => {
	printDataDemo.value = strData as string;
};

// Format and print test data
const formatPrintDataDemo = () => {
	try {
		const obj = JSON.parse(printDataDemo.value);
		printDataDemo.value = JSON.stringify(obj, null, 2);
	} catch (e) {
		ElMessageBox.alert('Error:' + e);
	}
};

// Export object
defineExpose({ hiprintTemplate, printDataDemo, setPrintDataDemo, initPaper, mode });
</script>

<style lang="scss" scoped>
.el-card {
    border-radius: 0;
}
:deep(.rect-printElement-types .hiprint-printElement-type > li > ul) {
    display: grid;
    grid-template-columns: 1fr 1fr;
    justify-content: center;
    justify-items: center;
    align-items: center;
    align-content: center;
    gap: 7px;

    li {
        width: 100%;
        a {
            height: auto;
            text-overflow: ellipsis;
            color: var(--el-color-primary);
            box-shadow: none !important;

            margin: 0;
            width: 100%;
        }
    }
}

// Default picture
:deep(.hiprint-printElement-image-content) {
	img {
		content: url('~@/assets/logo.png');
	}
}

:deep(.hiprint-option-item-submitBtn) {
	background: var(--el-color-primary);
}

:deep(.hiprint-option-item-deleteBtn) {
	background: var(--el-color-danger);
}

:deep(.prop-tabs .prop-tab-items li.active) {
	color: var(--el-color-primary);
	border-bottom: 2px solid var(--el-color-primary);
}
:deep(.el-tabs--border-card > .el-tabs__content) {
    overflow: auto;
    padding: 10px;
}
:deep(.hiprint-option-items) {
    padding: 0;
}
</style>
