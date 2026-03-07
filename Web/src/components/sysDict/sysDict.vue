<!-- Component usage documentation: https://gitee.com/zuohuaijun/Admin.NET/pulls/1559  -->
<script setup lang="ts">
import { reactive, watch, PropType } from 'vue';
import { useUserInfo } from '/@/stores/userInfo';

type DictItem = {
  [key: string]: any;
  tagType?: string;
  styleSetting?: string;
  classSetting?: string;
};

const userStore = useUserInfo();
const emit = defineEmits(['update:modelValue']);
const props = defineProps({
  /**
   * Boundvalue，Supports multipleType
   * @example
   * <g-sys-dict v-model="selectedValue" code="xxxx" />
   */
  modelValue: {
    type: [String, Number, Boolean, Array, null] as PropType<string | number | boolean | any[] | null>,
    default: null,
    required: true,
  },
  /**
   * Dictionary Encoding，used forObtaindictionaryitem
   * @example 'gender'
   */
  code: {
    type: String,
    required: true,
  },
  /**
   * YesnoYesConstant
   * @default false
   */
  isConst: {
    type: Boolean,
    default: false,
  },
  /**
   * dictionaryiteminused forDisplayofField Name
   * @default 'label'
   */
  propLabel: {
    type: String,
    default: 'label',
  },
  /**
   * dictionaryiteminused for takingvalueofField Name
   * @default 'value'
   */
  propValue: {
    type: String,
    default: 'value',
  },
  /**
   * dictionaryItem filter function
   * @param dict - dictionaryitem
   * @returns YesnoKeep this item
   * @default (dict) => true
   */
  onItemFilter: {
    type: Function as PropType<(dict: DictItem) => boolean>,
    default: (dict: DictItem) => true,
  },
  /**
   * dictionaryitemDisplaycontentFormatting function
   * @param dict - dictionaryitem
   * @returns FormattedDisplaycontent
   * @default () => undefined
   */
  onItemFormatter: {
    type: Function as PropType<(dict: DictItem) => string | undefined | null>,
    default: () => undefined,
  },
  /**
   * groupPiece rendering method
   * @values 'tag', 'select', 'radio', 'checkbox'
   * @default 'tag'
   */
  renderAs: {
    type: String as PropType<'tag' | 'select' | 'radio' | 'checkbox'>,
    default: 'tag',
    validator(value: string) {
      return ['tag', 'select', 'radio', 'checkbox'].includes(value);
    },
  },
  /**
   * YesnoMultiple Choice
   * @default false
   */
  multiple: {
    type: Boolean,
    default: false,
  },
});

const state = reactive({
  dict: undefined as DictItem | DictItem[] | undefined,
  dictData: [] as DictItem[],
  value: undefined as any,
});

// Get dataset
const getDataList = () => {
  if (props.isConst) {
    const data = userStore.constList?.find((x: any) => x.code === props.code)?.data?.result ?? [];
    // Consistent with the displayed text and values ​​of the dictionary to facilitate rendering
    data?.forEach((item: any) => {
      item.label = item.name;
      item.value = item.code;
      delete item.name;
    });
    return data;
  } else {
    return userStore.dictList[props.code];
  }
}

// Set dictionary data
const setDictData = () => {
  state.dictData = getDataList()?.filter(props.onItemFilter) ?? [];
  processNumericValues(props.modelValue);
};

// Handling numeric type values
const processNumericValues = (value: any) => {
  if (typeof value === 'number' || (Array.isArray(value) && typeof value[0] === 'number')) {
    state.dictData.forEach((item) => {
      item[props.propValue] = Number(item[props.propValue]);
    });
  }
};

// Set multiple selection values
const trySetMultipleValue = (value: any) => {
  let newValue = value;
  if (typeof value === 'string') {
    const trimmedValue = value.trim();
    if (trimmedValue.startsWith('[') && trimmedValue.endsWith(']')) {
      try {
        newValue = JSON.parse(trimmedValue);
      } catch (error) {
        console.warn('[g-sys-dict] Failed to parse multiple selection values, exception information:', error);
      }
    }
  } else if (props.multiple && !value) {
    newValue = [];
  }
  if (newValue != value) updateValue(newValue);

  setDictData();
  return newValue;
}

// Set dictionary value
const setDictValue = (value: any) => {
  value = trySetMultipleValue(value);
  if (Array.isArray(value)) {
    state.dict = state.dictData?.filter((x) => value.find(y => y == x[props.propValue]));
    state.dict?.forEach(ensureTagType);
  } else {
    state.dict = state.dictData?.find((x) => x[props.propValue] == value);
    if (state.dict) ensureTagType(state.dict);
  }
  state.value = value;
};

// Make sure the label type exists
const ensureTagType = (item: DictItem) => {
  if (!['success', 'warning', 'info', 'primary', 'danger'].includes(item.tagType ?? '')) {
    item.tagType = 'primary';
  }
};

// Update binding value
const updateValue = (newValue: any) => {
  emit('update:modelValue', newValue);
};

// Calculate displayed text
const getDisplayText = (dict: DictItem | undefined = undefined) => {
  if (dict) return props.onItemFormatter?.(dict) ?? dict[props.propLabel];
  return state.value;
}

watch(
    () => props.modelValue,
    (newValue) => setDictValue(newValue),
    { immediate: true }
);
</script>

<template>
  <!-- render tag -->
  <template v-if="props.renderAs === 'tag'">
    <template v-if="Array.isArray(state.dict)">
      <el-tag v-for="(item, index) in state.dict" :key="index" v-bind="$attrs" :type="item.tagType" :style="item.styleSetting" :class="item.classSetting" class="mr2">
        {{ getDisplayText(item) }}
      </el-tag>
    </template>
    <template v-else>
      <el-tag v-if="state.dict" v-bind="$attrs" :type="state.dict.tagType" :style="state.dict.styleSetting" :class="state.dict.classSetting">
        {{ getDisplayText(state.dict) }}
      </el-tag>
      <span v-else>{{ getDisplayText() }}</span>
    </template>
  </template>

  <!-- Render selector -->
  <template v-if="props.renderAs === 'select'">
    <el-select v-model="state.value" v-bind="$attrs" :multiple="props.multiple" @change="updateValue" clearable>
      <el-option v-for="(item, index) in state.dictData" :key="index" :label="getDisplayText(item)" :value="item[propValue]" />
    </el-select>
  </template>

  <!-- Render checkbox (multiple selection) -->
  <template v-if="props.renderAs === 'checkbox'">
    <el-checkbox-group v-model="state.value" v-bind="$attrs" @change="updateValue">
      <el-checkbox-button v-for="(item, index) in state.dictData" :key="index" :value="item[propValue]">
        {{ getDisplayText(item) }}
      </el-checkbox-button>
    </el-checkbox-group>
  </template>

  <!-- Render radio button -->
  <template v-if="props.renderAs === 'radio'">
    <el-radio-group v-model="state.value" v-bind="$attrs" @change="updateValue">
      <el-radio v-for="(item, index) in state.dictData" :key="index" :value="item[propValue]">
        {{ getDisplayText(item) }}
      </el-radio>
    </el-radio-group>
  </template>
</template>
<style scoped lang="scss">
</style>