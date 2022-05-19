import { RoleEnum } from "../_enum/RoleEnum";
import { RoleDto } from "../_models/API_Messages/RoleDto";


export class MenuEntry {
  name: string;
  longName?: string;            // default: none
  pageName: string;             // route page for menu entries, id for menu sections
  description?: string;         // default: none
  iconName?: string;            // default: none
  iconType?: MenuIconType;      // default: Material
  bigIcon?: boolean;            // default: false
  color?: MenuColor;            // default: Orange
  subMenus?: MenuEntry[];       // default: none
  roles?: RoleEnum[];           // default: all roles
  locations?: MenuLocation[];   // default: all locations
  section?: MenuSection;        // default: none
}

export class MenuSection {
  name: string;
  description?: string;
  iconName?: string;
}

export enum MenuColor {
  Purple = 'purple',
  Blue = 'blue',
  Red = 'red',
  Orange = 'orange',
  Green = 'green'
}

export enum MenuLocation {
  Navbar,
  Homepage,
  UserMenu
}

export enum MenuIconType {
  Material,
  Custom
}
