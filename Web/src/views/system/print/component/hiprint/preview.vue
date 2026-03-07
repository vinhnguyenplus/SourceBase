<template>
    <div class="preview-dialog" v-show="state.dialogVisible">
        <el-dialog v-model="state.dialogVisible" width="80%">
            <template #header>
                <div style="color: #fff">
                    <el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Printer /> </el-icon>
                    <span>{{ props.title }}</span>
                </div>
            </template>
            <div id="preview_content" ref="previewContentRef"></div>
            <template #footer>
                <el-button :loading="state.waitShowPrinter" type="primary" icon="ele-Printer" @click.stop="print">Print directly</el-button>
                <el-button type="primary" icon="ele-Printer" @click.stop="toPdf">ExportPDF</el-button>
                <el-button key="close" @click="hideDialog"> Close </el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script lang="ts" setup>
import { nextTick, reactive, ref } from 'vue';

var props = defineProps({
	title: {
		type: String,
		default: '',
	},
});

const state = reactive({
	dialogVisible: false,
	waitShowPrinter: false,
	width: 0, // Paper width mm
	printData: {}, // Print data
	printType: 1, // Default browser printing
	printParam: {
		printer: '', // Printer name
		title: '', // Print task name
		color: false, // Whether to print colors, default true
		copies: 1, // Number of copies to print Default 1
	},
	// Print parameters
	hiprintTemplate: {} as any,
});

const previewContentRef = ref();

const showDialog = (hiprintTemplate: any, printData: {}, width = 210, printType = 1, printParam: { printer: ''; title: ''; color: false; copies: 1 }) => {
	state.dialogVisible = true;
	state.width = width;
	state.hiprintTemplate = hiprintTemplate;
	state.printData = printData;
	state.printParam = printParam;
	state.printType = printType;
	nextTick(() => {
		while (previewContentRef.value?.firstChild) {
			previewContentRef.value.removeChild(previewContentRef.value.firstChild);
		}
		const newHtml = hiprintTemplate.getHtml(printData);
		previewContentRef.value.appendChild(newHtml[0]);
	});
};

const print = () => {
	state.waitShowPrinter = true;
	// debugger;
	// Determine whether the connection is successful
	if (state.printType == 2) {
		// Note: The connection is asynchronous
		// Connected
		// Get printer list
		const printerList = state.hiprintTemplate.getPrinterList();

		let sfcz = printerList.some((item: any) => {
			return item.name == state.printParam.printer;
		});
		if (!sfcz) {
			alert('Printer does not exist');
		} else {
			// Direct printing will use the default printer set by the system
			state.hiprintTemplate.print2(state.printData, state.printParam);

			// Sending task to printer successfully
			state.hiprintTemplate.on('printSuccess', function (e: any) {
				state.waitShowPrinter = false;
			});
			// Failed to send job to printer
			state.hiprintTemplate.on('printError', function (e: any) {
				state.waitShowPrinter = false;
				alert('Printing failed:' + e);
			});
		}
	} else {
		state.hiprintTemplate.print(
			state.printData,
			{},
			{
				callback: () => {
					state.waitShowPrinter = false;
				},
			}
		);
	}
};

const toPdf = () => {
	state.hiprintTemplate.toPdf(state.printData, 'PDF file');
};

const hideDialog = () => {
	state.dialogVisible = false;
};

defineExpose({ showDialog });
</script>

<style lang="scss" scoped>
.preview-dialog {
    :deep(.el-dialog) {
        display: flex;
        flex-direction: column;
        height: calc(100% - 20px);

        .el-dialog__body {
            background-color: var(--next-bg-main-color);
        }
    }

    :deep(.hiprint-printTemplate .hiprint-printPanel) {
        display: grid;
        grid-template-columns: 1fr;
        gap: 24px;
        
        .hiprint-printPaper {
            margin: auto;
            background-color: #fff;
            box-shadow: 5px 5px 10px 0px rgb(78, 78, 78);
        }
    }
}
</style>
