<template>
	<div class="sys-formDes-container">
		<v-form-designer ref="vFormDesignRef"></v-form-designer>
	</div>
</template>

<script lang="ts" setup name="sysFormDes">
import { ref } from 'vue';

const vFormDesignRef = ref(null);
</script>

<style lang="scss" scoped>
body {
	margin: 0; // Remove vertical scroll bar from page
}

.form-designer {
	overflow: unset !important;
}
</style>
