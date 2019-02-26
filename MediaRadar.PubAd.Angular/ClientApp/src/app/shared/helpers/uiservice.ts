import { Injectable } from "@angular/core";

import * as jsAction from '../../../assets/limitless/js/core/uiActions.js';

@Injectable()
export class UIService {

  hideTaskBar() {
    jsAction.hideSideBar();
  }

  reRegisterPlugins() {
    jsAction.reRegisterPlugins();
  }
  registerDatatables() {
    jsAction.registerDatatables();
  }
}
