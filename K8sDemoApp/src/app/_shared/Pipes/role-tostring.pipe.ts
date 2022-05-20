import { Pipe, PipeTransform } from '@angular/core';
import { RoleEnum } from 'src/app/_enum/RoleEnum';
import { RoleDto } from 'src/app/_models/API_Messages/RoleDto';

@Pipe({
  name: 'roleToShortString'
})
export class RoleToShortStringPipe implements PipeTransform {

  transform(value: RoleDto): string {
    switch(value.role) {
      case RoleEnum.admin:
        return 'A';
      case RoleEnum.standard:
        return 'S';
    }
    return '?';
  }

}

@Pipe({
  name: 'roleToLongString'
})
export class RoleToLongStringPipe implements PipeTransform {

  transform(value: RoleDto): string {
   return RoleEnum[value.role]
  }

}

