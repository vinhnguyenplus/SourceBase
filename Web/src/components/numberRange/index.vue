<!-- 
// NumberRange component is used to input a number range (supports prefix and suffix slots, supports numerical precision, and supports limiting value ranges). It is used in range input scenarios of values, amounts, etc.
// Usage example:
<template>
	<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10">
		<el-form-item label="Order amount">
			<number-range v-model="queryParams.amountRange">
				<template #prepend>
					<span>range</span>
				</template>
			</number-range>
		</el-form-item>
	</el-col>
</template>
<script lang="ts" setup>
import NumberRange from '/@/components/numberRange/index.vue';
</script>
-->

<template>
	<div class="number-range-container">
		<div :id="usePrepend ? 'prepend' : ''" :class="{ 'slot-default': slotStyle === 'default', 'slot-pend ': usePrepend }">
			<slot name="prepend">
				<!-- prefix slot -->
			</slot>
		</div>
		<div
			class="number-range"
			:class="{
				'is-disabled': disabled,
				'is-focus': isFocus,
				'number-range-left-border-radius-0': usePrepend,
				'number-range-right-border-radius-0': useAppend,
			}"
		>
			<el-input-number
				:disabled="disabled"
				placeholder="Minimum value"
				@blur="handleBlur"
				@focus="handleFocus"
				@change="handleChangeMinValue"
				@update:modelValue="updateMinValue"
				v-model="minValue_"
				v-bind="$attrs"
				:controls="false"
			/>
			<div class="to">
				<span>{{ to }}</span>
			</div>
			<el-input-number
				:disabled="disabled"
				placeholder="maximum value"
				@blur="handleBlur"
				@focus="handleFocus"
				@change="handleChangeMaxValue"
				@update:modelValue="updateMaxValue"
				v-model="maxValue_"
				v-bind="$attrs"
				:controls="false"
			/>
			<!-- clear icon -->
			<el-icon v-if="clearable && (minValue_ || maxValue_)" class="el-icon el-input__icon el-input__clear" @click="clearValues">
				<CircleClose />
			</el-icon>
		</div>
		<div :id="useAppend ? 'append' : ''" :class="{ 'slot-default': slotStyle === 'default', 'slot-pend ': useAppend }">
			<slot name="append">
				<!-- suffix slot -->
			</slot>
		</div>
	</div>
</template>
<script lang="ts" setup name="numberRange">
import { computed, ref, useSlots } from 'vue';
import { CircleClose } from '@element-plus/icons-vue';

const props = defineProps({
	modelValue: {
		type: Array<Number>,
		default: () => [null, null], // Use v-model="[min,max]" binding when calling
	},
	clearable: {
		type: Boolean,
		default: false,
	},
	minValue: {
		type: Number,
		default: null, // Use v-model:min-value="" when calling to bind multiple v-models
	},
	maxValue: {
		type: Number,
		default: null, // Use v-model:max-value="" when calling to bind multiple v-models
	},
	// Whether to disable
	disabled: {
		type: Boolean,
		default: false,
	},
	to: {
		type: String,
		default: '-',
	},
	// Precision parameter - number of decimal places to keep
	precision: {
		type: Number,
		default: 0,
		validator(val: number) {
			return val >= 0 && val === parseInt(String(val), 10);
		},
	},
	// Limit the value range
	valueRange: {
		type: Array,
		default: () => [],
		validator(val: []) {
			if (val && val.length > 0) {
				// @ts-ignore
				if (val.length !== 2) {
					throw new Error('Please pass in a Number array of length 2');
				}
				// @ts-ignore
				if (typeof val[0] !== 'number' || typeof val[1] !== 'number') {
					throw new Error('The range of values only accepts the Number type, please confirm');
				}
				// @ts-ignore
				if (val[1] < val[0]) {
					throw new Error('The valueRange format must be [minimum value, maximum value], please confirm');
				}
			}
			return true;
		},
	},
	// Slot style
	slotStyle: {
		type: String, // default --different color background | plain --no background color
		default: 'plain',
	},
});

const emit = defineEmits(['update:modelValue', 'update:minValue', 'update:maxValue', 'change']);

const minValue_ = computed({
	get() {
		return props.minValue || props.modelValue[0] || null;
	},
	set(value) {
		if (value === null) {
			emit('update:minValue', null);
			emit('update:modelValue', [null, maxValue_.value]);
			return;
		}
		emit('update:minValue', value);
		emit('update:modelValue', [value, maxValue_.value]);
	},
});

const maxValue_ = computed({
	get() {
		return props.maxValue || props.modelValue[1] || null;
	},
	set(value) {
		if (value === null) {
			emit('update:maxValue', null);
			emit('update:modelValue', [minValue_.value, null]);
			return;
		}
		emit('update:maxValue', value);
		emit('update:modelValue', [minValue_.value, value]);
	},
});

// How to clear a value
const clearValues = () => {
	minValue_.value = null;
	maxValue_.value = null;
	emit('update:modelValue', [null, null]);
};

const handleChangeMinValue = (value: number | null) => {
	// Returns null if it is not a number.
	if (value === null || isNaN(value)) {
		emit('update:minValue', null);
		return;
	}
	// Initialize numeric precision
	const newMinValue = parsePrecision(value, props.precision);
	// min > max swap min max
	if (typeof newMinValue === 'number' && parseFloat(String(newMinValue)) > parseFloat(String(maxValue_.value))) {
		// Value range determination
		const { min, max } = decideValueRange(Number(maxValue_.value), newMinValue);
		// Update binding value
		updateValue(min, max);
	}
};

const handleChangeMaxValue = (value: number | null) => {
	// Returns null if it is not a number.
	if (value === null || isNaN(value)) {
		emit('update:maxValue', null);
		return;
	}
	// Initialize numeric precision
	const newMaxValue = parsePrecision(value, props.precision);
	// max < min swap min max
	if (typeof newMaxValue === 'number' && parseFloat(String(newMaxValue)) < parseFloat(String(minValue_.value))) {
		// Value range determination
		const { min, max } = decideValueRange(newMaxValue, Number(minValue_.value));
		// Update binding value
		updateValue(min, max);
	}
};

const updateMinValue = (value: number | null) => {
	minValue_.value = value;
};

const updateMaxValue = (value: number | null) => {
	maxValue_.value = value;
};

// Update data
const updateValue = (min: number | null, max: number | null) => {
	emit('update:minValue', min);
	emit('update:maxValue', max);
	emit('update:modelValue', [min, max]);
	emit('change', { min, max });
};

// Value range determination
const decideValueRange = (min: number | null, max: number | null) => {
	if (min === null || max === null) {
		return { min, max };
	}

	if (props.valueRange && props.valueRange.length > 0) {
		// @ts-ignore
		min = min < props.valueRange[0] ? props.valueRange[0] : min > props.valueRange[1] ? props.valueRange[1] : min;
		// @ts-ignore
		max = max > props.valueRange[1] ? props.valueRange[1] : max;
	}
	return { min, max };
};

// input focus event
const isFocus = ref();

const handleFocus = () => {
	isFocus.value = true;
};

const handleBlur = () => {
	isFocus.value = false;
};

// Handling numerical precision
const parsePrecision = (number: number | null, precision = 0) => {
	if (number === null) {
		return null;
	}
	return parseFloat(String(Math.round(number * Math.pow(10, precision)) / Math.pow(10, precision)));
};

// Determine whether the slot is in use
// Inserted when the component is used externally
// <template #slot name>
// </template>
// Regardless of whether content is inserted into the template tag, the slot is considered to have been used.
const slots = useSlots();
const usePrepend = computed(() => {
	// prefix slot
	return slots && slots.prepend ? true : false;
});
const useAppend = computed(() => {
	// suffix slot
	return slots && slots.append ? true : false;
});
</script>
<style lang="scss" scoped>
.number-range-container {
	position: relative;
	display: flex;
	height: 100%;
	.slot-pend {
		white-space: nowrap;
		color: var(--el-color-info);
		border-radius: var(--el-input-border-radius, var(--el-border-radius-base));
	}
	#prepend {
		padding: 0 20px;
		box-shadow:
			1px 0 0 0 var(--el-input-border-color, var(--el-border-color)) inset,
			0 1px 0 0 var(--el-input-border-color, var(--el-border-color)) inset,
			0 -1px 0 0 var(--el-input-border-color, var(--el-border-color)) inset;
		border-right: 0;
		border-top-right-radius: 0;
		border-bottom-right-radius: 0;
	}
	#append {
		padding: 0 20px;
		box-shadow:
			0 1px 0 0 var(--el-input-border-color, var(--el-border-color)) inset,
			0 -1px 0 0 var(--el-input-border-color, var(--el-border-color)) inset,
			-1px 0 0 0 var(--el-input-border-color, var(--el-border-color)) inset;
		border-left: 0;
		border-top-left-radius: 0;
		border-bottom-left-radius: 0;
	}
	.slot-default {
		background-color: var(--el-fill-color-light);
	}

	.number-range-left-border-radius-0 {
		border-top-left-radius: 0 !important;
		border-bottom-left-radius: 0 !important;
	}
	.number-range-right-border-radius-0 {
		border-top-right-radius: 0 !important;
		border-bottom-right-radius: 0 !important;
	}

	.number-range {
		background-color: var(--el-bg-color) !important;
		box-shadow: 0 0 0 1px var(--el-input-border-color, var(--el-border-color)) inset;
		border-radius: var(--el-input-border-radius, var(--el-border-radius-base));
		padding: 0 2px;
		display: flex;
		flex-direction: row;
		width: 100%;
		justify-content: center;
		align-items: center;
		color: var(--el-input-text-color, var(--el-text-color-regular));
		transition: var(--el-transition-box-shadow);
		transform: translate3d(0, 0, 0);
		overflow: hidden;

		.to {
			margin-top: 1px;
		}
	}

	.is-focus {
		transition: all 0.3s;
		box-shadow: 0 0 0 1px var(--el-color-primary) inset !important;
	}
	.is-disabled {
		background-color: var(--el-input-bg-color);
		color: var(--el-input-text-color, var(--el-text-color-regular));
		cursor: not-allowed;
		.to {
			height: calc(100% - 3px);
			background-color: var(--el-fill-color-light) !important;
		}
	}
}

.el-input__clear {
	cursor: pointer;
	color: var(--el-input-icon-color, var(--el-text-color-placeholder));
	margin-left: 10px;
	display: none;
	position: absolute;
	right: 10px;
	top: 50%;
	transform: translateY(-50%);
}

.number-range:hover .el-input__clear {
	display: flex;
	align-items: center;
	color: var(--el-input-clear-hover-color);
}

:deep(.el-input) {
	border: none;
}
:deep(.el-input__wrapper) {
	margin: 0;
	padding: 0 15px;
	background-color: transparent;
	border: none !important;
	box-shadow: none !important;
	&.is-focus {
		border: none !important;
		box-shadow: none !important;
	}
}

:deep(.el-input),
:deep(.el-select),
:deep(.el-input-number) {
	width: 100%;
}
</style>
