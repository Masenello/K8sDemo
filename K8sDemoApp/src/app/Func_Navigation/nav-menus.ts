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
    name: 'Users Management',
    description: 'Manage user data',
    pageName: 'usermanagement',
    iconName: 'face',
    color: MenuColor.Green,
  },
  {
    name: 'System Test',
    pageName: '',
    description: 'Test system functions',
    iconName: 'science',
    color: MenuColor.Orange,
    subMenus: [
      {
        name: 'Jobs Director',
        pageName: 'asyncJobTest',
        description: 'Job Director test',
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
