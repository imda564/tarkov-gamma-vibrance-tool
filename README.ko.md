# 타르코프 감마·바이브런스 자동 조절 툴 (구 tarkov-settings-fixed)
![screenshot](./1.png)

**Language:** [English](README.md) | 한국어 | [Русский](README.ru.md) | [中文](README.zh.md) | [日本語](README.ja.md)

이 저장소는 [incheon-kim/tarkov-settings](https://github.com/incheon-kim/tarkov-settings)의 유지보수 포크입니다. 원본은 2023년 10월 이후 업데이트가 없고, 해결되지 않은 크래시 이슈가 여러 개 남아있습니다. 이 포크에서 수정한 내용은 다음과 같습니다:
- NVIDIA 디스플레이 핸들이 무효화될 때(원격 데스크톱 세션 전환, NVIDIA 제어판 닫기, 모니터 절전/핫플러그) 발생하던 크래시 — 원본 이슈 [#3](https://github.com/incheon-kim/tarkov-settings/issues/3), [#17](https://github.com/incheon-kim/tarkov-settings/issues/17) 참고
- 타이틀바 X 버튼으로 창을 닫을 때 설정이 저장되지 않던 문제 (기존에는 트레이 아이콘 → Exit 로만 저장됨)
- `settings.json`이 손상되거나 잘못된 경우 앱이 크래시하던 문제 (이제는 경고 후 기본값으로 리셋)
- 기본 감시 프로세스 목록에 `EscapeFromTarkovArena` 추가 — 원본 이슈 [#23](https://github.com/incheon-kim/tarkov-settings/issues/23) 참고

## [->**최신 버전 다운로드**<-](https://github.com/imda564/tarkov-gamma-vibrance-tool/releases/latest)

[Escape from Tarkov](https://escapefromtarkov.com)의 색상 설정을 자동으로 변경합니다.

## 작동 방식
- [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper)를 이용해 NVIDIA 설정의 디지털 바이브런스 값을 변경합니다
- [Win32 API 호출](https://docs.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-setdevicegammaramp)로 감마 값을 변경합니다

Escape from Tarkov 창이 포커스 상태일 때만 디스플레이 색상을 변경합니다.
창을 최소화/복원할 때 부드럽게 전환됩니다.

## 지원 그래픽 카드
- NVIDIA GPU **완전 지원** (밝기/대비/감마/채도)
- AMD GPU **부분 지원** (채도 제외)
- **Intel/기타는 아직 지원하지 않습니다.**

## 무엇을 할 수 있나요?
다음 색상 설정을 변경할 수 있습니다:
1. 밝기 (Brightness)
2. 대비 (Contrast)
3. 감마 (Gamma)
4. 디지털 바이브런스 컨트롤 (채도)
5. EFT 창이 포커스 상태일 때만 적용됨 (알트탭 시 화면이 갑자기 번쩍이는 현상도 방지)

## 사용 방법
1. 앱 실행 (서명되지 않은 프로그램이라 SmartScreen이 경고할 수 있습니다)
2. 원하는 색상 값 설정
2.1. 슬라이더 라벨을 더블클릭하면 값이 초기화됩니다.
3. 앱을 최소화하고 EFT 플레이
4. 비활성화하려면 앱을 종료하세요

**창을 닫거나 트레이 아이콘에서 종료할 때 `settings.json`에 설정이 저장됩니다.**

## 프로필
좌측 하단 입력창에 이름을 입력하고 **Save** / **Load** / **Del** 버튼으로 밝기/대비/감마/채도 값을 여러 세트로 저장하고 불러올 수 있습니다 (예: 게임별, 시간대별 프로필). 프로필은 다른 설정과 함께 `settings.json`에 저장됩니다.

## 주의사항
1. EFT 창을 활성화할 때 화면이 몇 번 깜빡일 수 있지만 정상 동작이니 걱정하지 마세요.
2. **면책조항: 이 프로그램 사용으로 BSG의 제재를 받을지 여부는 보장할 수 없습니다.**
3. AMD는 밝기/대비/감마 컨트롤만 지원합니다.
4. Intel 그래픽 카드는 지원하지 않습니다.
5. **테두리 없음(Borderless) 모드**에서만 동작합니다.
6. NVIDIA Optimus 환경(대부분의 노트북)은 테스트되지 않았습니다.

## TODO / 기능
- [x] 프로세스 포커스 감지
- [x] 디지털 바이브런스 값 변경
- [x] 감마 값 변경
- [x] 밝기/대비/감마 값 수정
- [x] GUI
- [x] ini 또는 json 설정
- [x] 프로세스 변경 가능 (EscapeFromTarkov 외에도 적용 가능)
- [x] 디스플레이(모니터) 대상 변경
- [x] 트레이로 최소화
- [x] 프로필
- [ ] 단축키
- [ ] EFT 설정 변경 (프레임 제한 또는 그래픽 설정)

응원해 주셔서 감사합니다!
