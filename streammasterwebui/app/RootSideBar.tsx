import MenuItemSM from '@components/MenuItemSM';
import {
  FilesEditorIcon,
  HelpIcon,
  LogIcon,
  PlayListEditorIcon,
  QueueStatisIcon,
  SDChannelIcon,
  SDIcon,
  SettingsEditorIcon,
  SideBarMenuIcon,
  StreamGroupEditorIcon,
  StreamingStatusIcon
} from '@lib/common/icons';
import { GetIsSystemReady } from '@lib/smAPI/Settings/SettingsGetAPI';
import useSettings from '@lib/useSettings';
import { useLocalStorage } from 'primereact/hooks';
import { Tooltip } from 'primereact/tooltip';
import { useCallback, useEffect, useState } from 'react';
import { Menu, MenuItem, Sidebar, sidebarClasses } from 'react-pro-sidebar';

export const RootSideBar = () => {
  const [collapsed, setCollapsed] = useLocalStorage<boolean>(true, 'app-menu-collapsed');
  const [isReady, setIsReady] = useState(false);

  // const getIsSystemReady = useSettingsGetIsSystemReadyQuery(undefined, { pollingInterval: 1000 * 1 });

  const settings = useSettings();

  useEffect(() => {
    const intervalId = setInterval(() => {
      GetIsSystemReady()
        .then((result) => {
          setIsReady(result ?? false);
        })
        .catch(() => {
          setIsReady(false);
        });
    }, 1000);

    return () => clearInterval(intervalId);
  }, []);

  const onsetCollapsed = useCallback(
    (isCollapsed: boolean) => {
      setCollapsed(isCollapsed);
    },
    [setCollapsed]
  );

  return (
    <Sidebar
      className="app sidebar max-h-screen "
      defaultCollapsed={collapsed}
      rootStyles={{
        [`.${sidebarClasses.container}`]: {
          backgroundColor: 'var(--mask-bg)'
        }
      }}
      style={{ height: 'calc(100vh - 10px)' }}
    >
      <Menu
        menuItemStyles={{
          button: ({ active }) => ({
            '&:hover': {
              backgroundColor: '#cb5e00'
            },
            backgroundColor: active ? '#cb5e00' : undefined
          })
        }}
      >
        <div
          onClick={() => {
            onsetCollapsed(!collapsed);
          }}
        >
          <MenuItem icon={<SideBarMenuIcon sx={{ color: 'var(--orange-color)', fontSize: 32 }} />}>
            <h2 className="orange-color">Stream Master</h2>
          </MenuItem>
        </div>
        {/* <MenuItemSM collapsed={collapsed} icon={<PlayListEditorIcon />} link="/testpanel" name='Test Panel' /> */}
        <MenuItemSM collapsed={collapsed} icon={<PlayListEditorIcon />} link="/editor/playlist" name="Playlist" />
        <MenuItemSM collapsed={collapsed} icon={<StreamGroupEditorIcon />} link="/editor/streamgroup" name="Stream Group" />
        <MenuItemSM collapsed={collapsed} icon={<FilesEditorIcon />} link="/editor/files" name="Files" />
        {settings.data.sdSettings?.sdEnabled === true ? (
          <MenuItemSM collapsed={collapsed} icon={<SDIcon />} link="/editor/sdHeadEndLineUps" name="SD HeadEnds" />
        ) : null}
        {settings.data.sdSettings?.sdEnabled === true ? (
          <MenuItemSM collapsed={collapsed} icon={<SDChannelIcon />} link="/editor/sdChannels" name="SD Channels" />
        ) : null}
        <MenuItemSM collapsed={collapsed} icon={<StreamingStatusIcon />} link="/streamingstatus" name="Status" />
        <MenuItemSM collapsed={collapsed} icon={<QueueStatisIcon />} link="/queuestatus" name="Queue" />
        <MenuItemSM collapsed={collapsed} icon={<SettingsEditorIcon />} link="/settings" name="Settings" />
        <MenuItemSM collapsed={collapsed} icon={<LogIcon />} link="/viewer/logviewer" name="Log" />
        <MenuItemSM collapsed={collapsed} icon={<HelpIcon />} link="https://github.com/SenexCrenshaw/StreamMaster/wiki" name="Wiki" newWindow />
      </Menu>

      <div className="absolute bottom-0 left-0 pb-2 flex flex-column m-0 p-0 justify-content-center align-items-center">
        <div className="flex col-12 justify-content-center align-items-center">
          <img alt="Stream Master Logo" src={isReady ? '/images/StreamMasterx32Ready.png' : '/images/StreamMasterx32NotReady.png'} />
        </div>
        <Tooltip target=".custom-target-icon" />
        <div
          className="custom-target-icon flex flex-column m-0 p-0 justify-content-center align-items-center text-xs text-center"
          data-pr-position="right"
          data-pr-tooltip={settings.data.release ?? ''}
        >
          {settings.data.version ?? ''}
        </div>
      </div>
    </Sidebar>
  );
};
