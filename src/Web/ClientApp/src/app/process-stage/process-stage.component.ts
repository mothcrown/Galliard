import { Component, Input } from '@angular/core';
import { ProcessStage} from "./process-stage";

@Component({
  selector: 'app-process-stage',
  templateUrl: './process-stage.component.html',
  styleUrls: ['./process-stage.component.scss']
})

export class ProcessStageComponent {
  @Input() stage: ProcessStage;

  constructor() {}

  setMessage(message: string) {
    this.stage.message = message;
  }

  startLoading() {
    this.stage.loading = true;
  }

  stopLoading() {
    this.stage.loading = false;
  }

  isSuccess() {
    this.stage.success = true;
  }

  hasFailed() {
    this.stage.success = false;
  }
}
