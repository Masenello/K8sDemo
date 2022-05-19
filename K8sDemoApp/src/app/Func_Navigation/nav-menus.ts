import { RoleEnum } from '../_enum/RoleEnum';
import { RoleDto } from '../_models/API_Messages/RoleDto';
import { MenuColor, MenuEntry, MenuIconType, MenuLocation, MenuSection } from './menu-entry.model';

export const settingsSection: MenuSection =
{
  name: 'Settings',
  description: 'Manage application settings',
  iconName: 'settings'
}

export const testsSection: MenuSection = {
  name: 'Tests',
  description: 'Application tests environment',
  iconName: 'science'
}

export const appMenus: MenuEntry[] = [
  {
    name: 'Home',
    description: 'Home page',
    pageName: 'home',
    iconName: 'home',
    color: MenuColor.Green,
  },
  {
    name: 'Users Management',
    description: 'Manage user data',
    pageName: 'usermanagement',
    iconName: 'face',
    color: MenuColor.Green,
    roles: [RoleEnum.admin],
  },
  {
    name: 'System Test',
    pageName: 'systemArchitecture',
    description: 'Test system functions',
    iconName: 'science',
    color: MenuColor.Orange,
    roles: [RoleEnum.admin],
    subMenus: [
      {
        name: 'Jobs Director',
        pageName: 'asyncJobTest',
        description: 'Job Director test',
        iconName: 'construction',
        color: MenuColor.Orange,
        roles: [RoleEnum.admin],
      },
    ]
  },
  {
    name: 'Cluster Monitoring',
    description: 'Monitor cluster status',
    pageName: 'clusterMonitoring',
    iconName: 'monitor_heart',
    color: MenuColor.Green,
    roles: [RoleEnum.admin],
  }
  
]
