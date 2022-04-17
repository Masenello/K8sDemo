import { Pipe, PipeTransform } from '@angular/core';
import { JobStatusEnum } from '../_enum/JobStatusEnum';
import { JobTypeEnum } from '../_enum/JobTypeEnum';

@Pipe({name: 'jobStatusEnumNamePipe'})
export class JobStatusEnumNamePipe implements PipeTransform {
  transform(value: JobStatusEnum): string {
    return JobStatusEnum[value];
  }
}

@Pipe({name: 'jobTypeEnumNamePipe'})
export class JobTypeEnumNamePipe implements PipeTransform {
  transform(value: JobTypeEnum): string {
    return JobTypeEnum[value];
  }
}