import { Pipe, PipeTransform } from '@angular/core';
import { LogMessage } from 'ngx-log-monitor';
import { ForwardLogMessage } from '../_models/Hub_Messages/ForwardLogMessage';
import { LogUtils } from '../_shared/Utils/LogUtils';

@Pipe({
  name: 'logViewerPipe'
})
export class LogViewerPipe implements PipeTransform {

  transform(value: ForwardLogMessage): LogMessage {
    let logUtils : LogUtils = new LogUtils; 
    return {
        timestamp: logUtils.ExtractDateString(value.message),
        message: logUtils.ExtractMessage(value.message),
        type:logUtils.ConvertLogType(value.messageType)
    }
  }


}
