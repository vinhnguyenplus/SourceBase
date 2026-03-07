<template>
	<div style="display: flex;">
        <!-- <div class="table-search-container"> -->
        <div :class="['table-search-container', { 'table-search-flex': search.length > defaultShowCount }]">
            <el-form ref="tableSearchRef" :model="state.innerModelValue" :inline="true" label-width="100px">
                <span v-for="(val, key) in search" :key="key" v-show="key < defaultShowCount || state.isToggle">
                    <template v-if="val.type">
                        <el-form-item
                                label-width="auto"
                                :label="val.label"
                                :prop="val.prop"
                                :rules="[{ required: val.required, message: `${val.label} cannot be empty`, trigger: val.type === 'input' ? 'blur' : 'change' }]"
                            >
                                <el-input
                                    v-model="state.innerModelValue[val.prop]"
                                    v-bind="val.comProps"
                                    :placeholder="val.placeholder"
                                    :clearable="!val.required"
                                    v-if="val.type === 'input'"
                                    @keyup.enter="onSearch(tableSearchRef)"
                                    @change="val.change"
                                />
                                <el-date-picker
                                    v-model="state.innerModelValue[val.prop]"
                                    v-bind="val.comProps"
                                    type="date"
                                    :placeholder="val.placeholder"
                                    :clearable="!val.required"
                                    v-else-if="val.type === 'date'"
                                    @change="val.change"
                                />
                                <el-date-picker
                                    v-model="state.innerModelValue[val.prop]"
                                    v-bind="val.comProps"
                                    type="monthrange"
                                    value-format="YYYY/MM/DD"
                                    :placeholder="val.placeholder"
                                    :clearable="!val.required"
                                    v-else-if="val.type === 'monthrange'"
                                    @change="val.change"
                                />
                                <el-date-picker
                                    v-model="state.innerModelValue[val.prop]"
                                    v-bind="val.comProps"
                                    type="daterange"
                                    value-format="YYYY/MM/DD"
                                    range-separator="to"
                                    start-placeholder="Start Date"
                                    end-placeholder="End Date"
                                    :clearable="!val.required"
                                    :shortcuts="shortcuts"
                                    :default-time="defaultTime"
                                    v-else-if="val.type === 'daterange'"
                                    @change="val.change"
                                />
                                <el-select
                                    v-model="state.innerModelValue[val.prop]"
                                    v-bind="val.comProps"
                                    :clearable="!val.required"
                                    :placeholder="val.placeholder"
                                    v-else-if="val.type === 'select'"
                                    @change="val.change"
                                >
                                    <el-option v-for="item in getSelectOptions(val)" :key="item.value" :label="item.label" :value="item.value" />
                                </el-select>
                                <el-cascader
                                    v-else-if="val.type === 'cascader' && val.cascaderData"
                                    :options="val.cascaderData"
                                    :clearable="!val.required"
                                    filterable
                                    :props="val.cascaderProps ? val.cascaderProps : state.cascaderProps"
                                    :placeholder="val.placeholder"
                                    @change="val.change"
                                    v-bind="val.comProps"
                                    v-model="state.innerModelValue[val.prop]"
                                >
                                </el-cascader>
                        </el-form-item>
                    </template>
                </span>
            </el-form>
        </div>
        <div v-if="search.length > defaultShowCount" class="table-search-more">
            <el-form :inline="true">
                <el-form-item>
                    <el-button text @click="state.isToggle = !state.isToggle"
                    >More queries
                        <el-icon class="el-icon--right">
                            <ele-ArrowUpBold v-if="state.isToggle" />
                            <ele-ArrowDownBold v-else />
                        </el-icon>
                </el-button>
                    
                    
                </el-form-item>
            </el-form>
        </div>
        <div class="table-search-btn">
            <el-form :inline="true">
                <el-form-item>
                    <!-- Using el-button-group will cause the right border of the button with type attribute to not be displayed -->
                    <!-- <el-button-group> -->
                    <el-button plain type="primary" icon="ele-Search" @click="onSearch(tableSearchRef)"> Query </el-button>
                    <el-button icon="ele-Refresh" @click="onReset(tableSearchRef)" style="margin-left: 12px"> reset </el-button>
                    <!-- </el-button-group> -->
                </el-form-item>
            </el-form>
        </div>
    </div>
</template>

<script setup lang="ts" name="makeTableDemoSearch">
import { reactive, ref, watch } from 'vue';
import type { FormInstance } from 'element-plus';
import { dayjs } from 'element-plus';
import {useUserInfo} from "/@/stores/userInfo";

// Define the value passed by the parent component
const props = defineProps({
	// Search form, type-control type (input, select, cascader, date), values ​​need to be passed when options-type is selct, cascaderData, cascaderProps-type need to be passed when type is cascader, the properties are the same as elementUI, if cascaderProps is not passed, use state by default.
	// The comProps attribute can be brought in, corresponding to the control attributes used.
	search: {
		type: Array<TableSearchType>,
		default: () => [],
	},
	modelValue: {
		type: Object,
		default: () => ({}),
	},
    // Several query conditions are displayed by default. If more than one are exceeded, they will be hidden. Click more to expand.
    defaultShowCount: {
        type: Number,
        default: 5,
    },
});

// Define child components to pass values/events to parent components
const emit = defineEmits(['search', 'reset', 'update:modelValue']);

// Define variable content
const tableSearchRef = ref<FormInstance>();
const state = reactive({
	isToggle: false,
	cascaderProps: { checkStrictly: true, emitPath: false, value: 'id', label: 'name', expandTrigger: 'hover' },
	/** internal modelValue */
	innerModelValue: {} as EmptyObjectType,
});

/** Monitor props.modelValue changes */
watch(
	() => props.modelValue,
	(val) => {
		state.innerModelValue = val;
	},
	{ immediate: true }
);

/** Monitor state.innerModelValue changes */
watch(
	() => state.innerModelValue,
	(val) => {
		emit('update:modelValue', val);
	},
	{ deep: true }
);

// Query
const onSearch = (formEl: FormInstance | undefined) => {
	if (!formEl) return;
	formEl.validate((isValid: boolean): void => {
		if (!isValid) return;

		emit('search', state.innerModelValue);
	});
};

// reset
const onReset = (formEl: FormInstance | undefined) => {
	if (!formEl) return;
	formEl.resetFields();
	emit('reset', state.innerModelValue);
};

const userStore = useUserInfo();
const getSelectOptions = (val: TableSearchType) => {
	if (val.options) return val.options;
	if (val.dictCode) return userStore.getDictDataByCode(val.dictCode);
	return [];
};

/** Time range default time */
const defaultTime = ref<[Date, Date]>([new Date(2000, 1, 1, 0, 0, 0), new Date(2000, 2, 1, 23, 59, 59)]);
/** Quick selection of time range */
const shortcuts = [
	{
		text: 'Within 7 days',
		value: () => {
			const end = dayjs().endOf('day').toDate();
			const start = dayjs().startOf('day').add(-7, 'day').toDate();
			return [start, end];
		},
	},
	{
		text: 'Within 1 month',
		value: () => {
			const end = dayjs().endOf('day').toDate();
			const start = dayjs().startOf('day').add(-1, 'month').toDate();
			return [start, end];
		},
	},
	{
		text: 'within 3 months',
		value: () => {
			const end = dayjs().endOf('day').toDate();
			const start = dayjs().startOf('day').add(-3, 'month').toDate();
			return [start, end];
		},
	},
];
</script>

<style scoped lang="scss">
.table-search-flex {
    flex: 1;
}

.table-search-container {
    //flex: 1;

    :deep(.el-form-item--small .el-form-item__label) {
        padding: 0 8px 0 0;
    }
}

.table-search-more {
	border-right: 1px solid var(--el-card-border-color);
}

.table-search-btn {
    flex-shrink: 0;

    // The query reset button on the right is vertically centered as it expands
    // .el-form--inline {
    //     height: 100%;
	// 	.el-form-item--small.el-form-item,.el-form-item:last-of-type {
    //         height: calc(100% - 10px);
    //     }
	// }
}
</style>
