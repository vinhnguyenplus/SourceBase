import type { App } from 'vue';
import { authDirective } from '/@/directive/authDirective';
import { wavesDirective, dragDirective, reclickDirective } from '/@/directive/customDirective';

/**
 * ExportInstruction Method：v-xxx
 * @methods authDirective UserPermission command，Usage：v-auth
 * @methods wavesDirective buttonWave command，Usage：v-waves
 * @methods dragDirective CustomizeDrag command，Usage：v-drag
 * @methods reclickDirective RepeatbuttonSubmit command，Usage：v-reclick
 */
export function directive(app: App) {
	// User permission instructions
	authDirective(app);
	// button wave command
	wavesDirective(app);
	// //Custom drag command
	// dragDirective(app);
	// Repeat button submit command
	reclickDirective(app);
}