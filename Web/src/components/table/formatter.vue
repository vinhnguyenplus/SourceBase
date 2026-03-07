<template>
	<!-- Write the render function variable in the temolate tag -->
	<render></render>
</template>

<script lang="ts" setup>
import { ref, watch, h } from 'vue';
// Define the value passed by the parent component
const props = defineProps<{
	fn: any;
}>();
const render = ref();
watch(
	props,
	async () => {
		render.value = h('div', null, props.fn);
	},
	{
		deep: true, // Confirm whether to monitor deeply
		immediate: true, // Confirm whether to execute the handler function with the current initial value
	}
);
</script>
