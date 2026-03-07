import '../lang/index'
import { createApp } from 'vue';
import pinia from '/@/stores/index';
import App from '/@/App.vue';
import router from '/@/router';
import { directive } from '/@/directive/index';
import other from '/@/utils/other';
import ElementPlus, { ElTooltip } from 'element-plus';
import '/@/theme/index.scss';
// animation library
import 'animate.css';
// grid layout
import VueGridLayout from 'vue-grid-layout';
// electronic signature
import VueSignaturePad from 'vue-signature-pad';
// Organization chart
import vue3TreeOrg from 'vue3-tree-org';
import 'vue3-tree-org/lib/vue3-tree-org.css';
// VForm3 form design
import VForm3 from 'vform3-builds';
import 'vform3-builds/dist/designer.style.css';
// Turn off automatic printing
import { disAutoConnect } from 'vue-plugin-hiprint';
import sysDict from "/@/components/sysDict/sysDict.vue";
import multiLangInput from "/@/components/multiLangInput/index.vue";
disAutoConnect();

const app = createApp(App);

directive(app);
other.elSvg(app);

// Register global dictionary component
app.component('GSysDict', sysDict);
// Register global multilingual components
app.component('GMultiLangInput', multiLangInput);

const TooltipProps = ElTooltip.props
TooltipProps.showAfter = { type: Number, default: 800 }; // Set the global tooltip delay display time to 800 milliseconds

app.use(pinia).use(router).use(ElementPlus).use(VueGridLayout).use(VForm3).use(VueSignaturePad).use(vue3TreeOrg).mount('#app');
