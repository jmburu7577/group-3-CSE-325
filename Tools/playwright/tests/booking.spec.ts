import { test, expect } from '@playwright/test';

test('patient can register, login and book appointment', async ({ page }) => {
  const base = 'http://localhost:5019';
  await page.goto(`${base}/Account/Register`);

  const email = `pwtest.${Date.now()}@example.test`;
  await page.fill('input[name="Input.FirstName"]', 'Play');
  await page.fill('input[name="Input.LastName"]', 'Writer');
  await page.fill('input[name="Input.Email"]', email);
  await page.fill('input[name="Input.Password"]', 'P@ssw0rd123!');
  await page.fill('input[name="Input.ConfirmPassword"]', 'P@ssw0rd123!');
  const dob = new Date(); dob.setFullYear(dob.getFullYear() - 30);
  const dobStr = dob.toISOString().slice(0,10);
  await page.fill('input[name="Input.DateOfBirth"]', dobStr);
  await page.fill('input[name="Input.PhoneNumber"]', '555-0123');
  await page.fill('input[name="Input.Address"]', '123 Test St');

  await Promise.all([
    page.waitForNavigation({ url: '**/' }),
    page.click('button[type="submit"]')
  ]);

  // Go to booking page
  await page.goto(`${base}/patient/book-appointment`);
  await expect(page).toHaveURL(/patient\/book-appointment/);

  // Select first specialty option (skip the placeholder)
  const specOptions = await page.$$eval('#specialty option', opts => opts.map(o => ({v: o.getAttribute('value'), t: o.textContent})).filter((o,i)=>i>0));
  if (specOptions.length === 0) test.fail(true, 'No specialties available');
  const specValue = specOptions[0].v as string;
  await page.selectOption('#specialty', specValue);

  // Wait for doctors to populate and select first available doctor
  await page.waitForSelector('#doctor option:not(:first-child)');
  const docOptions = await page.$$eval('#doctor option', opts => opts.map(o => o.getAttribute('value')).filter(v=>v));
  if (docOptions.length === 0) test.fail(true, 'No doctors available');
  const doctorValue = docOptions[0] as string;
  await page.selectOption('#doctor', doctorValue);

  // Set date/time and reason
  const d = new Date(); d.setDate(d.getDate()+3);
  const dateStr = d.toISOString().slice(0,10);
  await page.fill('#appointmentDate', dateStr);
  await page.fill('#appointmentTime', '09:00');
  await page.fill('#reason', 'Playwright automated booking');

  // Submit and wait for success indicator
  await Promise.all([
    page.waitForSelector('text=Appointment Booked Successfully!', { timeout: 15000 }),
    page.click('button[type="submit"]')
  ]);

  await expect(page.locator('text=Appointment Booked Successfully!')).toBeVisible();
});
