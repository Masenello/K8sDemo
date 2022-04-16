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
    name: 'Catalogs',
    pageName: 'catalogs',
    description: 'Manage catalogs',
    iconName: 'collections_bookmark',
    color: MenuColor.Orange,
    subMenus: [
      {
        name: 'Distributors',
        pageName: 'distributors',
        description: 'Manage distributors list',
        iconName: 'business',
        color: MenuColor.Orange,
      },
      {
        name: 'Connector Groups',
        description: 'Manage connector groups',
        pageName: 'connector-groups',
        iconName: 'group_work',
        color: MenuColor.Purple
      }
    ]
  },
  {
    name: 'Products',
    description: 'Manage products informations',
    pageName: 'products',
    iconName: 'widgets',
    color: MenuColor.Green,
    subMenus: [
      {
        name: 'Id-One',
        pageName: 'products_idone',
        iconName: 'ai-idone',
        iconType: MenuIconType.Custom,
        bigIcon: true,
        color: MenuColor.Blue,
        subMenus: [
          {
            name: 'Licenses',
            pageName: 'idone-licenses',
            description: 'Manage Id-One licenses',
            iconName: 'key',
            color: MenuColor.Blue
          }
          ,
          {
            name: 'Instruments',
            pageName: 'i-done-instruments',
            description: 'Manage I-dOne instruments',
            iconName: 'dns',
            color: MenuColor.Red
          }
        ]
      },
      {
        name: 'Molecular Mouse',
        pageName: 'products_molecularmouse',
        iconName: 'ai-molecularmouse',
        iconType: MenuIconType.Custom,
        bigIcon: true,
        color: MenuColor.Green,
        subMenus: [
          {
            name: 'Licenses',
            pageName: 'mouse-licenses',
            description: 'Manage Molecular Mouse licenses',
            iconName: 'key',
            color: MenuColor.Blue
          },
          {
            name: 'Mice web',
            pageName: 'mice-web',
            description: 'Manage Molecular Mouse connected',
            iconName: 'ai-molecularmouse',
            iconType: MenuIconType.Custom,
            color: MenuColor.Green
          }
          ,
          {
            name: 'Reagents',
            pageName: 'mouse_reagents',
            description: 'Manage Molecular Mouse cartridges',
            iconName: 'science',
            color: MenuColor.Purple,
            subMenus: [
              {
                name: 'Production',
                pageName: 'mouse-reagents-production',
                description: 'Molecular Mouse cartridges production',
                iconName: 'sd_card',
                color: MenuColor.Blue
              },
              {
                name: 'Catalog',
                pageName: 'mouse-reagents',
                description: 'Manage Molecular Mouse cartridges',
                iconName: 'science',
                color: MenuColor.Purple
              }
            ]
          }
        ]
      }
    ]
  },
  {
    name: 'Connectors',
    description: 'Manage AWM connectors',
    pageName: 'connectors',
    iconName: 'developer_board',
    color: MenuColor.Purple,
    roles: [ RoleEnum.Admin ]
  },
  {
    name: 'Alifax Software',
    description: 'Manage Alifax Software',
    pageName: 'alifax-software-list',
    iconName: 'ai-alifax-logo',
    iconType: MenuIconType.Custom,
    color: MenuColor.Red,
    roles: [RoleEnum.Admin ]
  },
  {
    name: 'Users',
    pageName: 'users',
    description: 'Manage application users',
    iconName: 'manage_accounts',
    color: MenuColor.Purple,
    roles: [ RoleEnum.Admin ],
    locations: [ MenuLocation.Homepage, MenuLocation.UserMenu ],
    section: settingsSection,
  },
  {
    name: 'SignalR Test',
    pageName: 'signalr-test',
    description: 'Open SignalR test environment',
    iconName: 'connect_without_contact',
    color: MenuColor.Blue,
    roles: [ RoleEnum.Admin ],
    section: testsSection,
  }
]
