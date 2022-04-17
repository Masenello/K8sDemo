import { MenuColor, MenuEntry, MenuIconType, MenuLocation, MenuSection } from './menu-entry.model';
import { RoleEnum } from '../_enum/RoleEnum';

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
    name: 'System Test',
    pageName: 'catalogs',
    description: 'Manage catalogs',
    iconName: 'quiz',
    color: MenuColor.Orange,
    subMenus: [
      {
        name: 'Database',
        pageName: 'databaseTest',
        description: 'Database test functions',
        iconName: 'storage',
        color: MenuColor.Orange,
        //roles: [ RoleEnum.Admin ]
      },
      {
        name: 'Async Jobs',
        pageName: 'asyncJobTest',
        description: 'Async jobs test functions',
        iconName: 'construction',
        color: MenuColor.Orange,
        //roles: [ RoleEnum.Admin ]
      },
    ]
  },
  {
    name: 'Cluster Monitoring',
    description: 'Monitor cluster status',
    pageName: 'clusterMonitoring',
    iconName: 'monitor_heart',
    color: MenuColor.Green,
  }
  
]
