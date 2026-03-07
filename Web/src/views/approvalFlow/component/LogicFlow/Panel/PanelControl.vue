<template>
	<div class="panel-control">
		<el-button-group>
			<el-button type="default" size="small" @click="$_zoomIn">Zoom in</el-button>
			<el-button type="default" size="small" @click="$_zoomOut">Reduce</el-button>
			<el-button type="default" size="small" @click="$_zoomReset">Size adaptation</el-button>
			<el-button type="default" size="small" @click="$_translateRest">Positioningrestore</el-button>
			<el-button type="default" size="small" @click="$_reset">Restore (Size & Position)</el-button>
			<el-button type="default" size="small" @click="$_undo" :disabled="state.undoDisable">Previous step (ctrl+z)</el-button>
			<el-button type="default" size="small" @click="$_redo" :disabled="state.redoDisable">Next step (Ctrl+Y)</el-button>
			<el-button type="default" size="small" @click="$_download">Download image</el-button>
			<el-button type="default" size="small" @click="$_catData">ViewData</el-button>
			<el-button type="default" size="small" @click="$_showMiniMap">View thumbnails</el-button>
		</el-button-group>
	</div>
</template>

<script setup lang="ts">
import { reactive } from 'vue';

var props = defineProps({
	lf: Object,
});
const emit = defineEmits(['catData']);

const state = reactive({
	undoDisable: true,
	redoDisable: true,
});

const $_zoomIn = () => {
	props.lf?.zoom(true);
};

const $_zoomOut = () => {
	props.lf?.zoom(false);
};

const $_zoomReset = () => {
	props.lf?.resetZoom();
};

const $_translateRest = () => {
	props.lf?.resetTranslate();
};

const $_reset = () => {
	props.lf?.resetZoom();
	props.lf?.resetTranslate();
};

const $_undo = () => {
	props.lf?.undo();
};

const $_redo = () => {
	props.lf?.redo();
};

const $_download = () => {
	props.lf?.getSnapshot();
};

const $_catData = () => {
	emit('catData');
};

const $_showMiniMap = () => {
	props.lf?.extension.miniMap.show(props.lf.graphModel.width - 210, 70);
};
</script>

<style lang="scss" scoped>
.panel-control {
	position: absolute;
	top: 30px;
	right: 50px;
	z-index: 2;
}
</style>
